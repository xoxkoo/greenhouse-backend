﻿using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class PresetEfcDao : IPresetDao
{
    private readonly Context _context;

    public PresetEfcDao(Context context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto parametersDto)
    {
        var listPreset = _context.Presets.AsQueryable();
        listPreset = listPreset.Include(p => p.Thresholds);

        if (parametersDto.IsCurrent != null)
        {
            if (parametersDto.IsCurrent == true)
            {
                listPreset = listPreset.Where(s => s.IsCurrent == parametersDto.IsCurrent);
            }
            else if (parametersDto.IsCurrent == false)
            {
                listPreset = listPreset.Where(s => s.IsCurrent == parametersDto.IsCurrent);
            }
        }

        if (parametersDto.Id != null)
        {
            listPreset = listPreset.Where(s => s.Id == parametersDto.Id);
        }

        IEnumerable<PresetDto> result = await listPreset.Select(p =>
            new PresetDto
            {
                Id = p.Id,
                Name = p.Name,
                IsCurrent = p.IsCurrent,
                Thresholds = p.Thresholds.Select(t =>
                    new ThresholdDto
                    {
	                    Type = t.Type,
                        Max = t.MaxValue,
                        Min = t.MinValue
                    })
            }).ToListAsync();

        return result;
    }

    public async Task<PresetEfcDto> CreateAsync(Preset preset)
    {
        if (preset == null)
        {
            throw new ArgumentNullException(nameof(preset), "Preset data cannot be null");
        }


        EntityEntry<Preset> entity = await _context.Presets.AddAsync(preset);
        await _context.SaveChangesAsync();


        IEnumerable<ThresholdDto> thresholdDtos = entity.Entity.Thresholds?.Select(t => new ThresholdDto
        {
            Type = t.Type,
            Min = t.MinValue,
            Max = t.MaxValue
        }) ?? Enumerable.Empty<ThresholdDto>();

        return new PresetEfcDto
        {
            Id = entity.Entity.Id,
            Name = entity.Entity.Name,
            Thresholds = thresholdDtos
        };
    }


    public async Task DeleteAsync(Preset preset)
    {

            var thresholds = _context.Thresholds.Where(t => t.PresetId == preset.Id);
            _context.Thresholds.RemoveRange(thresholds);
            _context.Presets.Remove(preset);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save changes to database", ex);
            }
    }

    public async Task<Preset?> GetByIdAsync(int id)
    {
        try
        {
            var list = _context.Presets.AsQueryable();
            list = list.Include(p => p.Thresholds);
            Preset? preset = await list.Where(p => p.Id == id).FirstOrDefaultAsync();


            if (preset == null)
            {
                return null;
            }

            return preset;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }



    // public async Task UpdateAsync(Preset preset)
    // {
    //     SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(preset.Id, null);
    //     IEnumerable<Threshold> existingThresholdsForPreset = GetAsync(parametersDto).Result.FirstOrDefault()?.Thresholds;
    //     _context.Thresholds.RemoveRange(existingThresholdsForPreset);
    //     await _context.Thresholds.AddRangeAsync(preset.Thresholds);
    //     _context.Presets.Update(preset);
    //
    //     await _context.SaveChangesAsync();
    // }

    public async Task ApplyAsync(int id)
    {
        //Changing previously applied preset value for isCurrent to be false
        var oldPresets = await _context.Presets.Where(p => p.IsCurrent == true).ToListAsync();
        foreach (var preset in oldPresets)
        {
            preset.IsCurrent = false;
            _context.Presets.Update(preset);
        }
        Preset presetToBeCurrent = await _context.Presets.FirstOrDefaultAsync(p => p.Id == id);
        if (presetToBeCurrent == null)
        {
            throw new Exception($"Preset with id {id} not found");
        }
        presetToBeCurrent.IsCurrent = true;
        _context.Presets.Update(presetToBeCurrent);
        await _context.SaveChangesAsync();
    }


    public async Task<PresetEfcDto> UpdateAsync(PresetEfcDto dto)
    {
	    var existingPreset = await _context.Presets
		    .Include(p => p.Thresholds)
		    .FirstOrDefaultAsync(p => p.Id == dto.Id);

	    if (existingPreset == null)
	    {
		    throw new Exception($"Preset with id {dto.Id} not found");
	    }


	    // remove old thresholds
	    _context.Thresholds.RemoveRange(existingPreset.Thresholds);

	    // Update preset properties
	    existingPreset.Name = dto.Name;

	    var preset = MapPresetEntity(dto);
	    // add new thresholds
	    existingPreset.Thresholds = preset.Thresholds;

	    await _context.SaveChangesAsync();

	    return new PresetEfcDto()
	    {
		    Id = dto.Id,
		    Name = dto.Name,
		    Thresholds = dto.Thresholds
	    };

    }


    private Preset MapPresetEntity(PresetEfcDto dto)
    {
	    List<Threshold> thresholds = dto.Thresholds.Select(t => new Threshold
	    {
		    Type = t.Type,
		    MaxValue = t.Max,
		    MinValue = t.Min
	    }).ToList();

	    Preset preset = new Preset()
	    {
		    Id = dto.Id,
		    Name = dto.Name,
		    Thresholds = thresholds
	    };

	    return preset;
    }
}
