namespace Domain.DTOs.ScheduleDTOs;

public class ScheduleDto
{
    public int Id { get; set; }
    public IEnumerable<IntervalDto> Intervals { get; set; }
}
