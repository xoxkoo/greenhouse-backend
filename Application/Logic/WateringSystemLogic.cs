using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.Logic;

public class WateringSystemLogic : IWateringSystemLogic
{
    private readonly IWateringSystemDao _wateringSystemDao;
    private readonly Converter _converter;

    public WateringSystemLogic(IWateringSystemDao wateringSystemDao)
    {
        _wateringSystemDao = wateringSystemDao;
    }

    public async Task<ValveStateDto> CreateAsync(ValveStateCreationDto dto)
    {
        var entity = new ValveState()
        {
            Toggle = dto.Toggle
        };
        //in converter call socket USE STATECREATION NOT ENTITY
        // await _converter.ConvertToHex;
        return await _wateringSystemDao.CreateAsync(entity);
    }

    public async Task<ValveStateDto> GetAsync()
    {
        return await _wateringSystemDao.GetAsync();
    }
}