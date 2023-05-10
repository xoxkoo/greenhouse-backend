using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;


[ApiController]
[Route("[controller]")]
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
    [HttpPost]
    public async Task<ActionResult<PresetEfcDto>> CreateAsync([FromBody] PresetCreationDto dto)
    {
        try
        {
            PresetEfcDto created = await _logic.CreateAsync(dto);
            return Ok(created);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}