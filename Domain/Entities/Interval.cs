using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Interval
{
	[Key]
	public int Id { get; set; }
	public TimeSpan StartDate { get; set; }
	public TimeSpan EndDate { get; set; }
	public DayOfWeek DayOfWeek { get; set; }
}
