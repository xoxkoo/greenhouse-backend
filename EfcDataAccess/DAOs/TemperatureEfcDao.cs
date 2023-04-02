using Application.DaoInterfaces;

namespace EfcDataAccess.DAOs;

public class TemperatureEfcDao : ITemperatureDao
{
	private readonly Context _context;

	public TemperatureEfcDao(Context context)
	{
		_context = context;
	}
}
