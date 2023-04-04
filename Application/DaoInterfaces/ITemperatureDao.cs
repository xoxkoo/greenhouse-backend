using Domain.DTOs;
using Domain.Models;

namespace Application.DaoInterfaces;

public interface ITemperatureDao
{
    Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto);
}