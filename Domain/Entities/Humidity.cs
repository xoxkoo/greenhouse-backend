using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;


public class Humidity
{
	[Key]
	public int HumidityId { get; set; }

	public DateTime Date { get; set; }
	public int Value { get; set; }

	private Humidity()
	{}
}
