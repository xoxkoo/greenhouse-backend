using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Schedule
{
	[Key]
	public int Id { get; set; }
	public IEnumerable<Interval> Intervals { get; set; }
}
