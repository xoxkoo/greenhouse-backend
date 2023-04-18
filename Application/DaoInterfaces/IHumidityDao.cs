using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IHumidityDao
{
	Task<IEnumerable<HumidityDto>> GetHumidityAsync(SearchMeasurementDto searchMeasurement);
	Task<HumidityDto> CreateHumidityAsync(Humidity humidity);
}
