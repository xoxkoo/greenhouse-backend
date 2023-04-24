using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;

namespace Application.Logic;

public class CO2Logic : ICO2Logic
{
    private readonly ICO2Dao _co2Dao;

    public CO2Logic(ICO2Dao co2Dao)
    {
        _co2Dao = co2Dao;
    }
    
    public async Task<CO2Dto> CreateAsync(CO2CreateDto dto)
    {
        var entity = new CO2()
        {
            Date = dto.Date,
            Value = dto.Value
        };
        return await _co2Dao.CreateAsync(entity);
    }
    
    public Task<IEnumerable<CO2Dto>> GetAsync(SearchMeasurementDto dto)
    {
        if (dto.Current)
        {
            dto.StartTime = null;
            dto.EndTime = null;
        }
        if (dto.StartTime > dto.EndTime)
        {
            throw new Exception("Start date cannot be before the end date");
        }
        if (dto.EndTime==null && dto.Current == false)
        {
            dto.EndTime=DateTime.MaxValue;
        }
        if (dto.StartTime==null && dto.Current == false)
        {
            dto.EndTime=DateTime.MinValue;
        }

        return _co2Dao.GetAsync(dto);
    }


}