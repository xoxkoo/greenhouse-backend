namespace Domain.DTOs;

public class PresetEfcDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<ThresholdDto> Thresholds { get; set; }
}