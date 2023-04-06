using Domain.DTOs;

namespace Application.DaoInterfaces;

public interface ICO2Dao
{
    Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto);
}