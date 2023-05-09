using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;

namespace Application.Logic;

public class PresetLogic : IPresetLogic
{
    private readonly IPresetDao _presetDao;

    public PresetLogic(IPresetDao presetDao)
    {
        _presetDao = presetDao;
    }

    public Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto dto)
    {
        return _presetDao.GetAsync(dto);
    }
}