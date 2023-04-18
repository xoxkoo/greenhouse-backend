using Domain.DTOs;


namespace Application.DaoInterfaces;

public interface ITemperatureDao
{
    Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto);
}
