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
		if (temperature == null)
		{
			throw new ArgumentNullException(nameof(temperature), "Temperature object cannot be null");
		}
		if (temperature.Value < -50)
		{
			throw new ArgumentOutOfRangeException(nameof(temperature.Value), "Value of temperature cannot be below -50°C");
		}
		if (temperature.Value > 60)
		{
			throw new ArgumentOutOfRangeException(nameof(temperature.Value), "Value of temperature cannot be above 60°C");
		}
		if (temperature.Date > DateTime.Now)
		{
			throw new ArgumentOutOfRangeException(nameof(temperature.Date), "Date of temperature cannot be in the future");
		}

		EntityEntry<Temperature> entity = await _context.Temperatures.AddAsync(temperature);
		await _context.SaveChangesAsync();

		TemperatureDto dto = new TemperatureDto
		{
			Date = entity.Entity.Date,
			TemperatureId = entity.Entity.TemperatureId,
			Value = entity.Entity.Value
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
			.Select(t => new TemperatureDto(){Date = t.Date,TemperatureId = t.TemperatureId,Value = t.Value})
			.ToListAsync();
		return result;
	}
}
