using System.Collections;
using Application.DaoInterfaces;
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
                    new Threshold
                    {
                        Id = t.Id,
                        Type = t.Type,
                        MaxValue = t.MaxValue,
                        MinValue = t.MinValue
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
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save changes to database", ex);
        }

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

    public async Task UpdateAsync(Preset preset)
    {
        SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(preset.Id, null);
        IEnumerable<Threshold> existingThresholdsForPreset = GetAsync(parametersDto).Result.FirstOrDefault()?.Thresholds;
        _context.Thresholds.RemoveRange(existingThresholdsForPreset);
        await _context.Thresholds.AddRangeAsync(preset.Thresholds);
        _context.Presets.Update(preset);

        await _context.SaveChangesAsync();
    }

    public async Task ApplyAsync(int id)
    {
        Preset? preset = _context.Presets.FirstOrDefault(p => p.Id == id);
        preset.IsCurrent = true;
        _context.Presets.Update(preset);
        await _context.SaveChangesAsync();
    }


    // public async Task<PresetEfcDto> UpdateAsync(PresetEfcDto dto)
    // {
	   //  var preset = MapPresetEntity(dto);
	   //  // Retrieve the existing entity from the database using its primary key
	   //  var existingPreset = await _context.Presets.FindAsync(dto.Id);
    //
	   //  // Update the properties of the existing entity
	   //  existingPreset.Name = dto.Name;
	   //  existingPreset.Thresholds = preset.Thresholds;
    //
	   //  Console.WriteLine(dto.Thresholds.FirstOrDefault().Max);
    //
	   //  // Save the changes to the database
	   //  await _context.SaveChangesAsync();
    //
	   //  return new PresetEfcDto()
	   //  {
		  //   Id = dto.Id,
		  //   Name = dto.Name,
		  //   Thresholds = dto.Thresholds
	   //  };
    //
    // }
    //
    // private Preset MapPresetEntity(PresetEfcDto dto)
    // {
	   //  List<Threshold> thresholds = dto.Thresholds.Select(t => new Threshold
	   //  {
		  //   Type = t.Type,
		  //   MaxValue = t.Max,
		  //   MinValue = t.Min
	   //  }).ToList();
    //
	   //  Preset preset = new Preset()
	   //  {
		  //   Id = dto.Id,
		  //   Name = dto.Name,
		  //   Thresholds = thresholds
	   //  };
    //
	   //  return preset;
    // }
}
