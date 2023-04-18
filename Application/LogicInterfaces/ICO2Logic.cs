using Domain.DTOs;
using Domain.Entities;

namespace Application.LogicInterfaces;

public interface ICO2Logic
{
    public Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto);
    public Task<CO2CreateDto> SaveAsync(CO2CreateDto dto);
}