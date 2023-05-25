
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("measurements/co2")]
public class CO2Controller:ControllerBase
{
    private readonly ICO2Logic Logic;

    public CO2Controller(ICO2Logic logic)
    {
        Logic = logic;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CO2Dto>>> GetAsync(
	    [FromQuery] bool? current,
	    [FromQuery] DateTime? startTime = null,
	    [FromQuery] DateTime? endTime = null)
    {
	    try
	    {

		    if (current == null)
		    {
			    current = Request.Query.ContainsKey("current");
		    }

		    var parameters = new SearchMeasurementDto((bool)current, startTime, endTime);

		    var co2s = await Logic.GetAsync(parameters);
		    return Ok(co2s);
	    }
	    catch (Exception e)
	    {
		    Console.WriteLine(e);
		    return StatusCode(500, e.Message);
	    }
    }

    [HttpPost]
    public async Task<ActionResult<CO2Dto>> CreateAsync([FromBody]CO2CreateDto dto)
    {
        try
        {
            CO2Dto created = await Logic.CreateAsync(dto);
            return Created($"/co2s/{created.CO2Id}", created);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
