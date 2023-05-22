namespace Domain.DTOs;

public class IntervalDto
{
	public int Id { get; init; }
	public TimeSpan StartTime { get; set; }
	public TimeSpan EndTime { get; set; }
	public DayOfWeek DayOfWeek { get; set; }
}
