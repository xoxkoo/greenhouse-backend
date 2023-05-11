using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
}