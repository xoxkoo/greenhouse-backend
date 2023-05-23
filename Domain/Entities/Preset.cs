using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Preset
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsCurrent { get; set; }
    public IEnumerable<Threshold> Thresholds { get; set; }

}