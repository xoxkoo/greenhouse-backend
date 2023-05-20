namespace Domain.DTOs.CreationDTOs;
using DTOs;
public class ScheduleCreationDto
{
    public IEnumerable<IntervalDto> Intervals { get; set; }
}
