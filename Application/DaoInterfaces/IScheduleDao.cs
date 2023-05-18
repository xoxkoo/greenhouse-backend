using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IScheduleDao
{
    Task<IEnumerable<IntervalDto>> CreateAsync(IEnumerable<Interval> intervals);
    Task<IEnumerable<IntervalDto>> GetAsync();
    Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek);
    Task PutAsync(IntervalDto dto);
    Task<IntervalDto> GetByIdAsync(int id);
    Task DeleteAsync(int id);

}
