using Domain.DTOs;
using Domain.Entities;

namespace Application.LogicInterfaces;

public interface IPresetLogic
{
    Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto dto);
}