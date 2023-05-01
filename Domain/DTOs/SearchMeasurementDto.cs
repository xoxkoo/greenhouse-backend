namespace Domain.DTOs;

public class SearchMeasurementDto
{
    public DateTime? StartTime;
    public DateTime? EndTime;
    public readonly bool Current;

    public SearchMeasurementDto(bool current, DateTime? startTime = null, DateTime? endTime = null)
    {
        StartTime = startTime;
        EndTime = endTime;
        Current = current;
    }
}
