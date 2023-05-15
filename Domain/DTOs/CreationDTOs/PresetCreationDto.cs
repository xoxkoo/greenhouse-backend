using Domain.Entities;

namespace Domain.DTOs.CreationDTOs;

public class PresetCreationDto
{
    public string Name { get; set; }
    public IEnumerable<ThresholdDto> Thresholds { get; set; }
}