using Domain.DTOs;

namespace Application.DaoInterfaces;

public interface IHumidityDao
{
	Task<IEnumerable<HumidityDto>> GetHumidityAsync(SearchMeasurementDto searchMeasurement);
}
