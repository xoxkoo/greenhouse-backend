
using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IWateringSystemDao
{
    Task<ValveStateDto> CreateAsync(ValveState entity);
    Task<ValveStateDto> GetAsync();
}
