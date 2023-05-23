using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("/humidity")]
public class HumidityController:ControllerBase
{
    private readonly IHumidityLogic _logic;

    public HumidityController(IHumidityLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HumidityDto>>> GetAsync([FromQuery] bool current, [FromQuery] DateTime? startTime = null,[FromQuery] DateTime? endTime = null)
    { 
        try
        {
            SearchMeasurementDto parameters = new SearchMeasurementDto(current, startTime,endTime);
            var humidities = await _logic.GetAsync(parameters);

            return Ok(humidities);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
