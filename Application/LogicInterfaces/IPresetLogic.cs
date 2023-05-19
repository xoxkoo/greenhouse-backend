using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.LogicInterfaces;

public interface IPresetLogic
{
    Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto dto);
    Task<PresetEfcDto> CreateAsync(PresetCreationDto dto);
    Task<PresetEfcDto> UpdateAsync(PresetEfcDto dto);
    Task ApplyAsync(int id); }