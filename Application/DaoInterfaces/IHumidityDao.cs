using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IHumidityDao
{
	Task<IEnumerable<HumidityDto>> GetAsync(SearchMeasurementDto searchMeasurement);
	Task<HumidityDto> CreateAsync(Humidity humidity);
}
