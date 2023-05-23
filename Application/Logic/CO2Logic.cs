using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
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
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "CO2 data cannot be null.");
        }

        if (dto.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Value), "CO2 value cannot be negative.");
        }
        if (dto.Value > 4095)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Value), "CO2 value cannot be bigger than 4095 ppm.");
        }
        if (dto.Date > DateTime.Now)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Date), "Date of temperature cannot be in the future");
        }

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
        else if (dto.StartTime > dto.EndTime)
        {
            throw new Exception("Start date cannot be before the end date");
        }
        else if (dto.EndTime==null && dto.Current == false)
        {
            dto.EndTime=DateTime.MaxValue;
        }
        else if (dto.StartTime==null && dto.Current == false)
        {
            dto.StartTime=DateTime.MinValue;
        }
        return _co2Dao.GetAsync(dto);
    }


}
