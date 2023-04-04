namespace Domain.DTOs;

public class TemperatureDto
{
    public float value{ get; set; }
    public DateTime Date { get; set; }

    public TemperatureDto(float value, DateTime date)
    {
        this.value = value;
        Date = date;
    }
}