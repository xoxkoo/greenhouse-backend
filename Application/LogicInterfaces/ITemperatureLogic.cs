using Domain.DTOs;
using Domain.DTOs.CreationDTOs;


namespace Application.LogicInterfaces;

public interface ITemperatureLogic
{
    public Task<TemperatureDto> CreateAsync(TemperatureCreateDto dto);

    public Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto);
}