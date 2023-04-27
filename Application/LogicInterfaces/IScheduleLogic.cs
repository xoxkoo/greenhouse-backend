using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

namespace Application.LogicInterfaces;

public interface IScheduleLogic
{
    Task<ScheduleDto> CreateAsync(ScheduleCreationDto dto);
}