using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;


namespace Application.DaoInterfaces;

public interface ITemperatureDao
{
    Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto);
    public Task<TemperatureDto> CreateAsync(Temperature temperature);
}
