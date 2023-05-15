using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IPresetDao
{
    Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto parametersDto);
    Task<PresetEfcDto> CreateAsync(Preset preset);
    Task UpdateAsync(Preset preset);
    Task ApplyAsync(int id);


}