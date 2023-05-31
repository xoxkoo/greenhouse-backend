using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ValveState
{
	[Key]
	public int Id { get; set; }
	public bool Toggle { get; set; }
}
