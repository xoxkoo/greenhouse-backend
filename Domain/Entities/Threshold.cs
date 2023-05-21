using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Threshold
{
    [Key]
    public int Id { get; set; }
    public string Type { get; set; }
    public float MinValue { get; set; }
    public float MaxValue { get; set; }
    public int PresetId { get; set; }
}