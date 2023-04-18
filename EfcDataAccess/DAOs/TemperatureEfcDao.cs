using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
		else if (dto.EndTime !=null &&dto.StartTime != null)
		{
			tempQuery = tempQuery.Where(t => t.Date >= dto.StartTime && t.Date <= dto.EndTime).AsQueryable() ;
		}
		
		IEnumerable<TemperatureDto> result = await tempQuery
			.Select(t => new TemperatureDto(t.Value, t.Date))
			.ToListAsync();
		return result;
	}
}
