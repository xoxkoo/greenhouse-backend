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
    public async Task<ActionResult<IEnumerable<TemperatureDto>>> GetAsync([FromQuery] bool current, [FromQuery] DateTime? startTime = null,[FromQuery] DateTime? endTime = null)
    {
        try
        {
            SearchMeasurementDto parameters = new SearchMeasurementDto(current, startTime,endTime);
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
