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
            MinValue = t.MinValue,
            MaxValue = t.MaxValue
        }) ?? Enumerable.Empty<ThresholdDto>();

        return new PresetEfcDto
        {
            Id = entity.Entity.Id,
            Name = entity.Entity.Name,
            Thresholds = thresholdDtos
        };
    }
}