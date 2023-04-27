using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

namespace Application.LogicInterfaces;

public interface IScheduleLogic
{
    Task<ScheduleDto> CreateAsync(ScheduleCreationDto dto);
    Task<IEnumerable<ScheduleDto>> GetAsync();
    Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek);

}