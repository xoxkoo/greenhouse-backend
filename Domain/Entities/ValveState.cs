using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class ValveState
{
	[Key]
	public bool Toggle { get; set; }
}
