using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("watering-system/toggle")]
public class WateringSystemController:ControllerBase
{
    private readonly IWateringSystemLogic Logic;

    public WateringSystemController(IWateringSystemLogic logic)
    {
        Logic = logic;
    }
    /** Creates a new object ValveStateCreationDto that is sent to the logic, the query returns ok message*/
    [HttpPost]
    public async Task<ActionResult<ValveStateDto>> PostAsync([FromBody] ValveStateCreationDto dto)
    {
        try
        {
            await Logic.CreateAsync(dto);
            return Ok();
        }
        catch (ArgumentException e)
        {
	        Console.WriteLine(e);
	        return StatusCode(400, e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<ValveStateDto>> GetAsync()
    {
        try
        {
            ValveStateDto state = await Logic.GetAsync();
            return Ok(state);
        }
        catch (ArgumentException e)
        {
	        Console.WriteLine(e);
	        return StatusCode(400, e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
