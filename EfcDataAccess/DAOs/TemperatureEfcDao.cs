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
	
	public async Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto searchMeasurement)
	{
		var list = _context.Temperatures.AsQueryable();
		long secondsPrecision = TimeSpan.TicksPerSecond;

		// if current is requested, return just last
		if (searchMeasurement.Current)
		{
			list = list
				.OrderByDescending(h => h.Date)
				.Take(1);
		}
		// return temperatures in interval
		// DateTime of temperatures saved in database is converted to Ticks (the number of ticks that have elapsed since January 1, 0001, at 00:00:00.000 in the Gregorian calendar.)
		// One tick is 0.0001 millisecond, we divide it by number of ticks in one second so that the precision is in seconds.
		else if (searchMeasurement.StartTime != null && searchMeasurement.EndTime != null)
		{
			list = list.Where(c => c.Date.Ticks/secondsPrecision >= searchMeasurement.StartTime.Value.Ticks/secondsPrecision-1 && c.Date.Ticks/secondsPrecision  <= searchMeasurement.EndTime.Value.Ticks/secondsPrecision);
		}
		else if (searchMeasurement.StartTime != null)
		{
			list = list.Where(c => c.Date.Ticks/secondsPrecision  >= searchMeasurement.StartTime.Value.Ticks/secondsPrecision-1 ).AsQueryable();
			
		}
		else if (searchMeasurement.EndTime != null)
		{
			list = list.Where(c => c.Date.Ticks/secondsPrecision  <= searchMeasurement.EndTime.Value.Ticks/secondsPrecision-1 ).AsQueryable();
		}

		IEnumerable<TemperatureDto> result = await list.Select(c =>
			new TemperatureDto()
			{
				Date = c.Date,
				TemperatureId = c.TemperatureId,
				value = c.Value
			}).ToListAsync();

		return result;
	}


}
