using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;


[ApiController]
public class PresetController : ControllerBase
{
    private readonly IPresetLogic _logic;

    public PresetController(IPresetLogic logic)
    {
        _logic = logic;
    }
    
    [Route("preset")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PresetDto>>> GetAsync()
    {
        try
        {
            SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(null, null);
            var presets = await _logic.GetAsync(parametersDto);
            return Ok(presets);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    
    [Route("preset-current")]
    [HttpGet]
    public async Task<ActionResult<PresetDto>> GetCurrentAsync()
    {
        try
        {
            SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(null, true);
            var presets = await _logic.GetAsync(parametersDto);
            var preset = presets.FirstOrDefault();
            return Ok(preset);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
    
    
}