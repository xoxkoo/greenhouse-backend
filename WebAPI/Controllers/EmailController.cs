using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace WebAPI.Controllers;

[ApiController]
[Route("/email")]
public class EmailController : ControllerBase
{
    private readonly IEmailLogic _logic;

    public EmailController(IEmailLogic logic)
    {
        _logic = logic;
    }

    [HttpPost]
    public async Task<ActionResult<EmailDto>> CreateAsync([FromBody] EmailDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Email data cannot be null");
        }
        try
        {
            EmailDto created = await _logic.CreateAsync(dto);
            return Created("/email", created);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<EmailDto>> GetAsync()
    {
        try
        {
            var email = await _logic.GetAsync();
            return Ok(email);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}