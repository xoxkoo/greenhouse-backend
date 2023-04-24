using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;


public class Temperature
{
	[Key]
	public int TemperatureId { get; set; }
	public DateTime Date { get; set; }
	public float Value { get; set; }
	
}
