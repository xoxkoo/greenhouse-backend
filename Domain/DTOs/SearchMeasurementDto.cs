namespace Domain.DTOs;

public class SearchMeasurementDto
{
    public DateTime? StartTime;
    public DateTime? EndTime;
    public Boolean Current;

    public SearchMeasurementDto(DateTime? startTime, DateTime? endTime, bool current)
    {
        StartTime = startTime;
        EndTime = endTime;
        Current = current;
    }
}
