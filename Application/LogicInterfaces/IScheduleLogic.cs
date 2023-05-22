using Domain.DTOs;

namespace Application.LogicInterfaces;

public interface IScheduleLogic
{
    Task<IEnumerable<IntervalDto>> CreateAsync(IEnumerable<IntervalDto> dto);
    Task<IEnumerable<IntervalDto>> GetAsync();
    Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek);
    Task PutAsync(IntervalDto dto);
    Task DeleteAsync(int id);
}
