using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;

namespace Application.Logic;

public class CO2Logic : ICO2Logic
{
    private readonly ICO2Dao _co2Dao;

    public CO2Logic(ICO2Dao co2Dao)
    {
        _co2Dao = co2Dao;
    }

    public Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto)
    {
        if (dto.current)
        {
            dto.startTime = null;
            dto.endTime = null;
        }
        if (dto.startTime > dto.endTime)
        {
            throw new Exception("Start date cannot be before the end date");
        }
        if (dto.endTime==null && dto.current == false)
        {
            dto.endTime=DateTime.MaxValue;
        }
        if (dto.startTime==null && dto.current == false)
        {
            dto.endTime=DateTime.MinValue;
        }

        return _co2Dao.GetAsync(dto);
    }
}