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
		DateTime startTime = searchMeasurement.StartTime ?? DateTime.MinValue; // Use DateTime.MinValue if StartTime is not provided
		DateTime endTime = searchMeasurement.EndTime ?? DateTime.MaxValue; // Use DateTime.MaxValue if EndTime is not provided

		var list = _context.Temperatures.AsQueryable();

		// if current is requested, return just last
		if (searchMeasurement.Current)
		{
			list = list
				.OrderByDescending(h => h.Date)
				.Take(1);
		}
		else
		{
			list = _context.Temperatures.Where(c => c.Date >= startTime && c.Date <= endTime);
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
