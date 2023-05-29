using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.Logic;

public class ValveLogic : IValveLogic
{
	private readonly IWateringSystemDao _wateringSystemDao;

	public ValveLogic(IWateringSystemDao wateringSystemDao)
	{
		_wateringSystemDao = wateringSystemDao;
	}
	public async Task<ValveStateDto> SetAsync(ValveStateCreationDto dto)
	{
		var entity = new ValveState()
		{
			Toggle = dto.State
		};

		return await _wateringSystemDao.CreateAsync(entity);
	}
}
