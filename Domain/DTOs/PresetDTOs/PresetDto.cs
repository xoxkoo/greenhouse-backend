using Domain.Entities;

namespace Domain.DTOs;

public class PresetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsCurrent { get; set; }
    public IEnumerable<Threshold> Thresholds { get; set; }
    
}