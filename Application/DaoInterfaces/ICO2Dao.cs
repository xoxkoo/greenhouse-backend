using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface ICO2Dao
{
    Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto);
    Task<CO2> SaveAsync(CO2 dto);

}
