using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CO2Controller:ControllerBase
{
    private readonly ICO2Logic Logic;

    public CO2Controller(ICO2Logic logic)
    {
        Logic = logic;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CO2Dto>>> GetAsync([FromQuery] Boolean current, [FromQuery] DateTime? startTime = null,[FromQuery] DateTime? endTime = null)
    {
        try
        {
	        SearchMeasurementDto parameters = new SearchMeasurementDto(current, startTime, endTime);
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
