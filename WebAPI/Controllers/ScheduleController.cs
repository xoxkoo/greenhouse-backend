using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

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
            return Ok(intervalDtos);
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
            await Logic.PutAsync(intervalDto);
            return Ok();
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}
