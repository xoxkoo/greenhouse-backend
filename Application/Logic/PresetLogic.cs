using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;

namespace Application.Logic;

public class PresetLogic : IPresetLogic
{
    private readonly IPresetDao _presetDao;
    public PresetLogic(IPresetDao presetDao)
    {
        _presetDao = presetDao;
    }

    public async Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto dto)
    {
        return await _presetDao.GetAsync(dto);
    }


}