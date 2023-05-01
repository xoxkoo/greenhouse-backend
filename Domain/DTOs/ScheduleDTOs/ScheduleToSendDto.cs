namespace Domain.DTOs;

public class ScheduleToSendDto
{
	public IEnumerable<IntervalToSendDto> Intervals { get; set; }
}
