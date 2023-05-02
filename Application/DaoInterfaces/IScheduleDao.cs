using Domain.DTOs;
using Domain.DTOs.ScheduleDTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IScheduleDao
{
    Task<ScheduleDto> CreateAsync(Schedule schedule);
    Task<IEnumerable<ScheduleDto>> GetAsync();
    Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek);

}
