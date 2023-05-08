

using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
// using Socket;

namespace Application.Logic;

public class WateringSystemLogic : IWateringSystemLogic
{
    private readonly IWateringSystemDao _wateringSystemDao;
    // private readonly IWebSocketCLient _webSocketClient;
    private readonly Converter _converter;


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
        if (dto.Toggle.Equals(null))
        {
            throw new Exception("Toggle has to be set");
        }
        if (dto.Toggle.Equals(true)&&(dto.duration<=0))
        {
            throw new Exception("Duration cannot be 0 or less");
        }
        var entity = new ValveState()
        {
            Toggle = dto.Toggle
        };

        var toggleDto = new ValveStateDto()
        {
	        Toggle = dto.Toggle
        };

        //in converter call socket USE STATECREATION NOT ENTITY
        string payload = _converter.ConvertActionsPayloadToHex(toggleDto, dto.duration);

        return await _wateringSystemDao.CreateAsync(entity);
    }

    public async Task<ValveStateDto> GetAsync()
    {
        return await _wateringSystemDao.GetAsync();
    }
}
