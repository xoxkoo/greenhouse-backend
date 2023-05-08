using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.LogicInterfaces;

public interface ICO2Logic
{
    //Testing
    public Task<CO2Dto> CreateAsync(CO2CreateDto dto);
    public Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto);
}