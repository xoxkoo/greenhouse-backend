using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

namespace Application.LogicInterfaces;

public interface IHumidityLogic
{
    public Task<HumidityDto> CreateAsync(HumidityCreationDto dto);

    public Task<IEnumerable<HumidityDto>> GetAsync(SearchMeasurementDto dto);
}
