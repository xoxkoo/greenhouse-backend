using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.LogicInterfaces;

public interface IPresetLogic
{
    Task<IEnumerable<PresetEfcDto>> GetAsync(SearchPresetParametersDto dto);
    Task<PresetEfcDto> CreateAsync(PresetCreationDto dto);
    Task<PresetEfcDto> UpdateAsync(PresetEfcDto dto);
    Task ApplyAsync(int id);
    Task<PresetEfcDto> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}
