using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TemperatureController:ControllerBase
{
    private readonly ITemperatureLogic Logic;

    public TemperatureController(ITemperatureLogic logic)
    {
        Logic = logic;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TemperatureDto>>> GetAsync([FromQuery] DateTime? startTime,[FromQuery] DateTime? endTime, [FromQuery] Boolean current)
    {
        try
        {
            SearchMeasurementDto parameters = new SearchMeasurementDto(startTime,endTime,current);
            var temperatures = await Logic.GetAsync(parameters);
            return Ok(temperatures);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
