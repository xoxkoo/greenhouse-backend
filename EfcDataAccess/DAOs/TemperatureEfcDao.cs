using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class TemperatureEfcDao : ITemperatureDao
{
	private readonly Context _context;

	public TemperatureEfcDao(Context context)
	{
		_context = context;
	}

	public async Task<TemperatureDto> CreateAsync(Temperature temperature)
	{

		EntityEntry<Temperature> entity = await _context.Temperatures.AddAsync(temperature);
		await _context.SaveChangesAsync();

		TemperatureDto dto = new TemperatureDto
		{
			Date = entity.Entity.Date,
			TemperatureId = entity.Entity.TemperatureId,
			value = entity.Entity.Value
		};
		return dto;
	}
	
	public async Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto)
	{
		IQueryable<Temperature> tempQuery= _context.Temperatures.AsQueryable();
		if (dto.Current)
		{
			tempQuery = tempQuery.OrderByDescending(t => t.Date).Take(1).AsQueryable();
		}
		if (dto.EndTime !=null && dto.StartTime != null)
		{
			tempQuery = tempQuery.Where(t => t.Date >= dto.StartTime && t.Date <= dto.EndTime).AsQueryable() ;
		}
		else if (dto.StartTime != null)
		{
			tempQuery = tempQuery.Where(t => t.Date >= dto.StartTime).AsQueryable();
		}
		else if (dto.EndTime != null)
		{
			tempQuery = tempQuery.Where(t => t.Date <= dto.EndTime).AsQueryable();
		}
		
		IEnumerable<TemperatureDto> result = await tempQuery
			.Select(t => new TemperatureDto(){Date = t.Date,TemperatureId = t.TemperatureId,value = t.Value})
			.ToListAsync();
		return result;
	}


}
