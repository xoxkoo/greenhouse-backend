

using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using SocketServer;


namespace Application.Logic;

public class WateringSystemLogic : IWateringSystemLogic
{
    private readonly IWateringSystemDao _wateringSystemDao;
    private readonly IConverter _converter;
    private readonly IWebSocketServer _socketServer;

    public WateringSystemLogic(IWateringSystemDao wateringSystemDao,IConverter converter,IWebSocketServer webSocketServer)
    {
        _wateringSystemDao = wateringSystemDao;
        _converter = converter;
        _socketServer = webSocketServer;
    }


    public async Task<ValveStateDto> CreateAsync(ValveStateCreationDto dto)
    {
       Validate(dto);

       var entity = new ValveState()
       {
	       Toggle = dto.State
       };

       string payload = _converter.ConvertActionsPayloadToHex(dto);

        await _socketServer.Connect();
        await _socketServer.Send(payload);
        await _socketServer.Disconnect();

        return await _wateringSystemDao.CreateAsync(entity);
    }

    public async Task<ValveStateDto> SetAsync(ValveStateCreationDto dto)
    {
	    Validate(dto);
	    var entity = new ValveState()
	    {
		    Toggle = dto.State
	    };

	    return await _wateringSystemDao.CreateAsync(entity);
    }

    private void Validate(ValveStateCreationDto dto)
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

    }

    public async Task<ValveStateDto> GetAsync()
    {
        return await _wateringSystemDao.GetAsync();
    }
}
