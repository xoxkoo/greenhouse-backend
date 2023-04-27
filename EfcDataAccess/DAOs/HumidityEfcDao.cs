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
		if (humidity == null)
		{
			throw new ArgumentNullException(nameof(humidity), "Humidity object cannot be null");
		}
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

		var list = _context.Humidities.AsQueryable();


		// if current is requested, return just last
		if (searchMeasurement.Current)
		{
			list = list
				.OrderByDescending(h => h.Date)
				.Take(1);
		}
		// return humidities in interval
		else if (searchMeasurement.StartTime != null && searchMeasurement.EndTime != null)
		{
			list = list.Where(h => h.Date >= searchMeasurement.StartTime && h.Date <= searchMeasurement.EndTime);
		}
		else if (searchMeasurement.StartTime != null)
		{
			list = list.Where(c => c.Date >= searchMeasurement.StartTime).AsQueryable();
			
		}
		else if (searchMeasurement.EndTime != null)
		{
			list = list.Where(c => c.Date <= searchMeasurement.EndTime).AsQueryable();
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
