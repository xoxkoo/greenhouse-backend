using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Interval
{
	[Key]
	public int Id { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public DayOfWeek DayOfWeek { get; set; }
}
