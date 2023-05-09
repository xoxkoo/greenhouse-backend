

using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;


namespace Application.Logic;

public class WateringSystemLogic : IWateringSystemLogic
{
    private readonly IWateringSystemDao _wateringSystemDao;
    private readonly IConverter _converter;


    public WateringSystemLogic(IWateringSystemDao wateringSystemDao)
    {
        _wateringSystemDao = wateringSystemDao;
    }

    public async Task<ValveStateDto> CreateAsync(ValveStateCreationDto dto)
    {
        if (dto.duration.Equals(null))
        {
            throw new Exception("duration has to be set");
        }
        if (dto.State.Equals(null))
        {
            throw new Exception("State has to be set");
        }
        if (dto.State.Equals(true)&&(dto.duration<=0))
        {
            throw new Exception("Duration cannot be 0 or less");
        }
        var entity = new ValveState()
        {
            Toggle = dto.State
        };

        var toggleDto = new ValveStateDto()
        {
	        State = dto.State
        };

        //in converter call socket USE STATECREATION NOT ENTITY
        // string payload = _converter.ConvertActionsPayloadToHex(toggleDto, dto.duration);

        return await _wateringSystemDao.CreateAsync(entity);
    }

    public async Task<ValveStateDto> GetAsync()
    {
        return await _wateringSystemDao.GetAsync();
    }
}
