using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
    public async Task<ActionResult<IEnumerable<CO2Dto>>> GetAsync([FromQuery] DateTime? startTime,[FromQuery] DateTime? endTime, [FromQuery] Boolean current)
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
}