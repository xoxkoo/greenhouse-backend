using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("measurements/humidity")]
public class HumidityController:ControllerBase
{
    private readonly IHumidityLogic _logic;

    public HumidityController(IHumidityLogic logic)
    {
        _logic = logic;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HumidityDto>>> GetAsync([FromQuery] bool? current, [FromQuery] DateTime? startTime = null,[FromQuery] DateTime? endTime = null)
    {
	    try
	    {
		    if (current == null)
		    {
			    current = Request.Query.ContainsKey("current");
		    }


		    var parameters = new SearchMeasurementDto((bool)current, startTime, endTime);
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
