using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.DTOs.ScheduleDTOs;
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
    public async Task<ActionResult<ScheduleDto>> CreateAsync([FromBody] IEnumerable<IntervalDto> intervals)
    {
	    Console.WriteLine(intervals.Count());
	    var dto = new ScheduleCreationDto()
	    {
		    Intervals = intervals
	    };

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<IntervalDto>>> GetAsync()
    {
        try
        {
            var schedules = await Logic.GetAsync();
            Console.WriteLine(schedules.FirstOrDefault().Intervals.Count());
            return Ok(schedules.FirstOrDefault().Intervals);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}
