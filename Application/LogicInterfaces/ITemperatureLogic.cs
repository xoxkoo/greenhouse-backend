using Domain.DTOs;
using Domain.Models;

namespace Application.LogicInterfaces;

public interface ITemperatureLogic
{
    public Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto);
}