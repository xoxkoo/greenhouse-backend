using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("/schedule")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleLogic Logic;

    public ScheduleController(IScheduleLogic logic)
    {
        Logic = logic;
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<IntervalDto>>> CreateAsync([FromBody] IEnumerable<IntervalToSendDto> intervals)
    {
        try
        {
            List<IntervalDto> intervalDtosToLogic = new List<IntervalDto>();
            foreach (var i in intervals)
            {
                var newInterval = new IntervalDto()
                {
                    DayOfWeek = i.DayOfWeek,
                    EndTime = i.EndTime,
                    StartTime = i.StartTime
                };
                intervalDtosToLogic.Add(newInterval);
            }
            IEnumerable<IntervalDto> intervalDtos = await Logic.CreateAsync(intervalDtosToLogic);
            return Created($"/intervals/{intervalDtos}", intervalDtos);
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
    public async Task<ActionResult<IEnumerable<IntervalDto>>> GetAsync()
    {
        try
        {
            var intervals = await Logic.GetAsync();
            return Ok(intervals);
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult> PutAsync([FromRoute] int id, [FromBody] IntervalDto intervalDto)
    {
        try
        {
	        if (id != intervalDto.Id)
	        {
		        return StatusCode(500, "Ids are not matching");
	        }
            await Logic.PutAsync(intervalDto);
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

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        try
        {
            await Logic.DeleteAsync(id);
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
}
