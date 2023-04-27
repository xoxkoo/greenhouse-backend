using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleLogic Logic;

    public ScheduleController(IScheduleLogic logic)
    {
        Logic = logic;
    }

    [HttpPost]
    public async Task<ActionResult<ScheduleDto>> CreateAsync([FromBody] ScheduleCreationDto dto)
    {
        try
        {
            ScheduleDto created = await Logic.CreateAsync(dto);
            return Created($"/schedule/{created.Id}", created);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}