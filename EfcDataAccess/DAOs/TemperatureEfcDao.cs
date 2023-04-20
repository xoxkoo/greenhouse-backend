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
	public async Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto)
	{
		IQueryable<Temperature> tempQuery= _context.Temperatures.AsQueryable();
		if (dto.Current)
		{
			tempQuery = tempQuery.OrderByDescending(t => t.Date).Take(1).AsQueryable();
		}
		if (dto.EndTime !=null &&dto.StartTime != null)
		{
			tempQuery = tempQuery.Where(t => t.Date >= dto.StartTime && t.Date <= dto.EndTime).AsQueryable() ;
		}
		Console.WriteLine(dto.EndTime);
		Console.WriteLine(dto.StartTime);
		Console.WriteLine(DateTime.Now);
		IEnumerable<TemperatureDto> result = await tempQuery
			.Select(t => new TemperatureDto(){Date = t.Date,value = t.Value})
			.ToListAsync();
		return result;
	}

	public async Task<TemperatureDto> SaveAsync(Temperature temperature)
	{
		EntityEntry<Temperature> entity = await _context.Temperatures.AddAsync(temperature);
		await _context.SaveChangesAsync();

		return new TemperatureDto()
		{
			Date = entity.Entity.Date,
			value = entity.Entity.Value
		};
	}
}
