namespace Domain.DTOs;

public class ScheduleDto
{
    public int Id { get; set; }
    public IEnumerable<IntervalDto> Intervals { get; set; }
}
