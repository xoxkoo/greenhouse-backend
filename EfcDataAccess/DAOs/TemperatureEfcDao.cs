using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;
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
		if (dto.endTime !=null &&dto.startTime != null)
		{
			tempQuery = tempQuery.Where(t => t.Date >= dto.startTime && t.Date <= dto.endTime).AsQueryable() ;
		}
		IEnumerable<TemperatureDto> result = await tempQuery
			.Select(t => new TemperatureDto(t.Value, t.Date))
			.ToListAsync();
		return result;
	}
}
