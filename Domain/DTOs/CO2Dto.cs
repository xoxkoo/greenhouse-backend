namespace Domain.DTOs;

public class CO2Dto
{
	public int CO2Id;

	public DateTime Date{ get; set; }
	public int Value{ get; set; }

	public CO2Dto(DateTime date, int value)
	{
		Date = date;
		Value = value;
	}
}
