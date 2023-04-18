using Domain.DTOs;

namespace Application.LogicInterfaces;

public interface ICO2Logic
{
    public Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto);
}