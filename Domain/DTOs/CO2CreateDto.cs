namespace Domain.DTOs;

public class CO2CreateDto
{
    public DateTime Date{ get; set; }
    public int Value{ get; set; }

    public CO2CreateDto(DateTime date, int value)
    {
        Date = date;
        Value = value;
    }
}