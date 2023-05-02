using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

namespace Application.LogicInterfaces;

public interface IHumidityLogic
{
    Task<HumidityDto> CreateAsync(HumidityCreationDto dto);

    Task<IEnumerable<HumidityDto>> GetAsync(SearchMeasurementDto dto);
}
