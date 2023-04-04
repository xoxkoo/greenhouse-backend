using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class CO2EfcDao : ICO2Dao
{
	private readonly Context _context;

	public CO2EfcDao(Context context)
	{
		_context = context;
	}

	public async Task<IEnumerable<HumidityDto>> GetHumidityAsync(SearchMeasurementDto searchMeasurement)
	{
		IEnumerable<Humidity> list = await _context.Humidities.ToListAsync();

		return list.Select(h => new HumidityDto
		{
			Date = h.Date,
			Value = h.Value,
			HumidityId = h.HumidityId
		});
	}
}
