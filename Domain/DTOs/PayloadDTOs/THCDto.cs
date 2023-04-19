namespace Domain.DTOs.PayloadDTOs;

public class THCDto
{
    public bool WaterOpen { get; set; }
    public float Temperature { get; set; }
    public int Humidity { get; set; }
    public int CO2 { get; set; }
    public DateTime DateTime { get; set; }
    
    
}