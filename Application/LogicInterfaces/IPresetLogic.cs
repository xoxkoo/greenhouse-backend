using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.LogicInterfaces;

public interface IPresetLogic
{
    Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto dto);
    Task<PresetEfcDto> CreateAsync(PresetCreationDto dto);
    Task DeleteAsync(int id);
    
    
}