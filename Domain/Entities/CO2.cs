using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class CO2
{
	[Key]
	public int CO2Id { get; set; }

	public DateTime Date { get; set; }
	public int Value { get; set; }

	private CO2()
	{}

	public CO2(DateTime date, int value)
	{
		Date = date;
		Value = value;
	}
}
