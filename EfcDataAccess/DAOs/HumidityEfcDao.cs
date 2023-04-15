using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class HumidityEfcDao : IHumidityDao
{
	private readonly Context _context;

	public HumidityEfcDao(Context context)
	{
		_context = context;
	}

	public async Task<IEnumerable<HumidityDto>> GetHumidityAsync(SearchMeasurementDto searchMeasurement)
	{
		IQueryable<Humidity> list = _context.Humidities.AsQueryable();

		// if current is requested, return just last
		if (searchMeasurement.Current)
		{
			list = _context.Humidities
				.OrderByDescending(h => h.Date)
				.Take(1);

		}

		// return humidities in interval
		if (searchMeasurement.StartTime != null && searchMeasurement.EndTime != null)
		{
			list = list.Where(h => h.Date >= searchMeasurement.StartTime && h.Date <= searchMeasurement.EndTime);

		}

		return await list.Select(h => new HumidityDto
		{
			Date = h.Date,
			Value = h.Value,
			HumidityId = h.HumidityId
		}).ToListAsync();
	}
}
