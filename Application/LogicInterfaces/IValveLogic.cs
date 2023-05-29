using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

namespace Application.LogicInterfaces;

public interface IValveLogic
{
	Task<ValveStateDto> SetAsync(ValveStateCreationDto dto);
}
