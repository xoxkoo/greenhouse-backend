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
    public async Task<ActionResult<IEnumerable<PresetEfcDto>>> GetAsync()
    {
        try
        {
            SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(null, null);
            var presets = await _logic.GetAsync(parametersDto);
            return Ok(presets);
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

    [Route("current-preset")]
    [HttpGet]
    public async Task<ActionResult<PresetEfcDto>> GetCurrentAsync()
    {
        try
        {
            SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(null, true);
            var presets = await _logic.GetAsync(parametersDto);
            var preset = presets.FirstOrDefault();
            return Ok(preset);
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

    [Route("preset")]
    [HttpPost]
    public async Task<ActionResult<PresetEfcDto>> CreateAsync([FromBody] PresetCreationDto dto)
    {
        try
        {
	        PresetEfcDto created = await _logic.CreateAsync(dto);

            return Created($"/preset/{created.Id}", created);
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

    [HttpPut("preset/{id:int}")]
    public async Task<ActionResult<PresetEfcDto>> UpdateAsync([FromRoute] int id, [FromBody] PresetEfcDto dto)
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


    //todo should be object wit id
    [HttpPost]
    [Route("current-preset")]
    public async Task<ActionResult> ApplyAsync([FromBody] int id)
    {
        try
        {
            await _logic.ApplyAsync(id);
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

    [Route("preset/{id:int}")]
    [HttpGet]
    public async Task<ActionResult<PresetEfcDto>> GetByIdAsync([FromRoute] int id)
    {
        try
        {
            var preset = await _logic.GetByIdAsync(id);
            return Ok(preset);
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
    [Route("preset/{id:int}")]
    [HttpDelete]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        try
        {
            await _logic.DeleteAsync(id);
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
