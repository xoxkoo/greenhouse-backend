using Application.DaoInterfaces;
namespace EfcDataAccess.DAOs;

public class CO2EfcDao : ICO2Dao
{
	private readonly Context _context;

	public CO2EfcDao(Context context)
	{
		_context = context;
	}
}
