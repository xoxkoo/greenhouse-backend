using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Interval
{
	[Key]
	public int Id { get; set; }
	public TimeSpan StartTime { get; set; }
	public TimeSpan EndTime { get; set; }
	public DayOfWeek DayOfWeek { get; set; }
}
