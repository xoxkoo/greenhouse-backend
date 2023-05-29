using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IWateringSystemLogic
{
    Task<ValveStateDto> CreateAsync(ValveStateCreationDto dto);
    Task<ValveStateDto> GetAsync();
}
