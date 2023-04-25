using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.Logic;

public class WateringSystemLogic : IWateringSystemLogic
{
    private readonly IWateringSystemDao _wateringSystemDao;
    

    public WateringSystemLogic(IWateringSystemDao wateringSystemDao)
    {
        _wateringSystemDao = wateringSystemDao;
    }

    public async Task<ValveStateDto> CreateAsync(ValveStateCreationDto dto)
    {
        var entity = new ValveState()
        {
            Toggle = dto.Toggle
        };
        //call converter
        //call socket 
        return await _wateringSystemDao.CreateAsync(entity);
    }
}