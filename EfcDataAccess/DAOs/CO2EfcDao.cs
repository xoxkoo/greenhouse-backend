using System.Runtime.InteropServices.JavaScript;
using System.Xml;
using Application.DaoInterfaces;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;

using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class CO2EfcDao : ICO2Dao
{
	private readonly Context _context;

	public CO2EfcDao(Context context)
	{
		_context = context;
	}

	public async Task<CO2Dto> CreateAsync(CO2 co2)
	{
		EntityEntry<CO2> entity = await _context.CO2s.AddAsync(co2);
		await _context.SaveChangesAsync();
		return new CO2Dto()
		{
			Date = entity.Entity.Date,
			CO2Id = entity.Entity.CO2Id,
			Value = entity.Entity.Value
		};
	}
	public async Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto searchMeasurement)
	{
		var list = _context.CO2s.AsQueryable();


		// if current is requested, return just last
		if (searchMeasurement.Current)
		{
			list = list
				.OrderByDescending(h => h.Date)
				.Take(1);
		}
		// return co2s in interval
		else if (searchMeasurement.StartTime != null && searchMeasurement.EndTime != null)
		{
			list = list.Where(c => c.Date >= searchMeasurement.StartTime && c.Date <= searchMeasurement.EndTime);
		}
		else if (searchMeasurement.StartTime != null)
		{
			list = list.Where(c => c.Date >= searchMeasurement.StartTime).AsQueryable();
			
		}
		else if (searchMeasurement.EndTime != null)
		{
			list = list.Where(c => c.Date <= searchMeasurement.EndTime).AsQueryable();
		}

		IEnumerable<CO2Dto> result = await list.Select(c =>
			new CO2Dto
			{
				Date = c.Date,
				Value = c.Value,
				CO2Id = c.CO2Id
			}).ToListAsync();

		return result;
	}


}
