using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
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

    [Route("current-preset")]
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
    [Route("preset")]
    [HttpPost]
    public async Task<ActionResult<PresetEfcDto>> CreateAsync([FromBody] PresetCreationDto dto)
    {
        try
        {
	        PresetEfcDto created = await _logic.CreateAsync(dto);

	        return Created($"/presets/{created.Id}", created);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("preset/{id:int}")]
    public async Task<ActionResult<PresetEfcDto>> UpdateAsync(int id, [FromBody] PresetEfcDto dto)
    {
	    try
	    {
		    if (id != dto.Id)
		    {
			    throw new Exception("Id does not match object!");
		    }

		    PresetEfcDto updated = await _logic.UpdateAsync(dto);

		    return Ok(updated);
	    }
	    catch (Exception e)
	    {
		    Console.WriteLine(e);
		    return StatusCode(500, e.Message);
	    }
    }

    [HttpPost]
    [Route("current-preset")]
    public async Task<ActionResult> ApplyAsync([FromBody] int id)
    {
	    try
	    {
		    await _logic.ApplyAsync(id);
		    return Ok();
	    }
	    catch (Exception e)
	    {
		    Console.WriteLine(e);
		    throw;
	    }
    }
}
