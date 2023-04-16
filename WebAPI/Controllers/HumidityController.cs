using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;
[ApiController]
[Route("[controller]")]

public class HumidityController:ControllerBase
{
    private readonly IHumidityLogic _logic;

    public HumidityController(IHumidityLogic logic)
    {
        _logic = logic;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HumidityDto>>> GetAsync([FromQuery] DateTime? startTime,[FromQuery] DateTime? endTime, [FromQuery] Boolean current)
    {
        try
        {
            SearchMeasurementDto parameters = new SearchMeasurementDto(startTime,endTime,current);
            var temperatures = await _logic.GetAsync(parameters);
            return Ok(temperatures);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}