using Domain.DTOs;


namespace Application.LogicInterfaces;

public interface ITemperatureLogic
{
    public Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto);
}
