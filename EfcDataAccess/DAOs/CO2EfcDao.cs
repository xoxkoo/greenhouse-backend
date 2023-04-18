using System.Runtime.InteropServices.JavaScript;
using System.Xml;
using Application.DaoInterfaces;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore;

using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class CO2EfcDao : ICO2Dao
{
	private readonly Context _context;

	public CO2EfcDao(Context context)
	{
		_context = context;
	}

	public async Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto)
	{
		IQueryable<CO2> tempQuery = _context.CO2s.AsQueryable();
		IEnumerable<CO2Dto> result;
		if (dto.EndTime !=null &&dto.StartTime != null && dto.Current != true)
		{
			tempQuery = tempQuery.Where(c => c.Date >= dto.StartTime && c.Date <= dto.EndTime).AsQueryable();
			result = await tempQuery
				.Select(c => new CO2Dto(c.Date, c.Value))
				.ToListAsync();
		}
		else if (dto.Current)
		{
			result = await tempQuery
				.Select(c => new CO2Dto(c.Date, c.Value))
				.ToListAsync();
			result = result.OrderByDescending(c => c.Date);
			result = result.Take(1).ToList();
		}
		else
		{
			result = await tempQuery
				.Select(c => new CO2Dto(c.Date, c.Value))
				.ToListAsync();
		}
		return result;
	}
}
