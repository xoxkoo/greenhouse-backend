using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
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
    public async Task<ActionResult<ValveStateDto>> PostAsync([FromBody] bool state, [FromBody] int time)
    {
        try
        {
            //figure out which dto to use
            ValveStateCreationDto valveStateCreationDto = new ValveStateCreationDto(){Toggle = state,duration = time};
            Logic.CreateAsync(valveStateCreationDto);
            return Ok(valveStateCreationDto);
        }
        catch (Exception e)
        {   
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}