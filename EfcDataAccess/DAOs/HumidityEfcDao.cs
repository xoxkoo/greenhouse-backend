using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class HumidityEfcDao : IHumidityDao
{
	private readonly Context _context;

	public HumidityEfcDao(Context context)
	{
		_context = context;
	}

	public async Task<HumidityDto> CreateAsync(Humidity humidity)
	{
		EntityEntry<Humidity> entity = await _context.Humidities.AddAsync(humidity);
		await _context.SaveChangesAsync();
		return new HumidityDto
		{
			Date = entity.Entity.Date,
			HumidityId = entity.Entity.HumidityId,
			Value = entity.Entity.Value
		};
	}

	public async Task<IEnumerable<HumidityDto>> GetAsync(SearchMeasurementDto searchMeasurement)
	{
		DateTime startTime = searchMeasurement.StartTime ?? DateTime.MinValue; // Use DateTime.MinValue if StartTime is not provided
		DateTime endTime = searchMeasurement.EndTime ?? DateTime.MaxValue; // Use DateTime.MaxValue if EndTime is not provided

		var list = _context.Humidities.AsQueryable();

		// if current is requested, return just last
		if (searchMeasurement.Current)
		{
			list = list
				.OrderByDescending(h => h.Date)
				.Take(1);
		}
		else
		{
			list = _context.Humidities.Where(c => c.Date >= startTime && c.Date <= endTime);
		}

		IEnumerable<HumidityDto> result = await list.Select(h =>
			new HumidityDto
			{
				Date = h.Date,
				Value = h.Value,
				HumidityId = h.HumidityId
			}).ToListAsync();

		return result;
	}

}
