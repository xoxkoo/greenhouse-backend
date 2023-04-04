namespace Domain.DTOs;

public class SearchMeasurementDto
{
    public DateTime? startTime;
    public DateTime? endTime;
    public Boolean current;

    public SearchMeasurementDto(DateTime? startTime, DateTime? endTime, bool current)
    {
        this.startTime = startTime;
        this.endTime = endTime;
        this.current = current;
    }
}