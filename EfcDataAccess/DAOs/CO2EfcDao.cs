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
		if (co2 == null)
		{
			throw new ArgumentNullException(nameof(co2), "CO2 data cannot be null.");
		}

		if (co2.Value < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(co2.Value), "CO2 value cannot be negative.");
		}
		if (co2.Value > 4095)
		{
			throw new ArgumentOutOfRangeException(nameof(co2.Value), "CO2 value cannot be bigger than 4095 ppm.");
		}
		if (co2.Date > DateTime.Now)
		{
			throw new ArgumentOutOfRangeException(nameof(co2.Date), "Date of temperature cannot be in the future");
		}

		EntityEntry<CO2> entity = await _context.CO2s.AddAsync(co2);
		await _context.SaveChangesAsync();
		return new CO2Dto()
		{
			Date = entity.Entity.Date,
			CO2Id = entity.Entity.CO2Id,
			Value = entity.Entity.Value
		};
	}
	
	public async Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto)
	{
		IQueryable<CO2> tempQuery = _context.CO2s.AsQueryable();
		IEnumerable<CO2Dto> result;
		if (dto.EndTime !=null &&dto.StartTime != null && dto.Current != true)
		{
			tempQuery = tempQuery.Where(c => c.Date >= dto.StartTime && c.Date <= dto.EndTime).AsQueryable();
			result = await tempQuery
				.Select(c => new CO2Dto(){CO2Id = c.CO2Id, Date = c.Date, Value = c.Value})
				.ToListAsync();
		}
		else if (dto.Current)
		{
			result = await tempQuery
				.Select(c => new CO2Dto(){CO2Id = c.CO2Id, Date = c.Date, Value = c.Value})
				.ToListAsync();
			result = result.OrderByDescending(c => c.Date);
			result = result.Take(1).ToList();
		}
		else if (dto.StartTime != null)
		{
			tempQuery = tempQuery.Where(c => c.Date >= dto.StartTime).AsQueryable();
			result = await tempQuery
				.Select(c => new CO2Dto() { CO2Id = c.CO2Id, Date = c.Date, Value = c.Value })
				.ToListAsync();
		}
		else if (dto.EndTime != null)
		{
			tempQuery = tempQuery.Where(c => c.Date <= dto.EndTime).AsQueryable();
			result = await tempQuery
				.Select(c => new CO2Dto() { CO2Id = c.CO2Id, Date = c.Date, Value = c.Value })
				.ToListAsync();
		}
		else
		{
			result = await tempQuery
				.Select(c => new CO2Dto(){CO2Id = c.CO2Id, Date = c.Date, Value = c.Value})
				.ToListAsync();
		}
		return result;
	}


}
