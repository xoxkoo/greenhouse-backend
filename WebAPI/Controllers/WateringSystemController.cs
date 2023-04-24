using Application.DaoInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WateringSystemController:ControllerBase
{
    private readonly IWateringSystemLogic Logic;

    public WateringSystemController(IWateringSystemLogic logic)
    {
        Logic = logic;
    }

    [HttpPost]
    public async Task<ActionResult<ValveStateDto>> PostAsync([FromBody] bool current, [FromBody] int time)
    {
        try
        {
            ValveStateDto valveStateDto = new ValveStateDto();
            return Ok(valveStateDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}