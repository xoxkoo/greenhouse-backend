namespace Domain.DTOs;

public class PresetCreationDto
{
    public string Name { get; set; }
    public IEnumerable<ThresholdDto> Thresholds { get; set; }
}