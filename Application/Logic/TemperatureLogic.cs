using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Models;

namespace Application.Logic;

public class TemperatureLogic : ITemperatureLogic
{
    private readonly ITemperatureDao _temperatureDao;

    public TemperatureLogic(ITemperatureDao temperatureDao)
    {
        _temperatureDao = temperatureDao;
    }

    public Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto)
    {
        if (dto.startTime>dto.endTime)
        {
            throw new Exception("Start date cannot be before the end date");
        }

        if (dto.endTime==null)
        {
            dto.endTime=DateTime.MaxValue;
        }
        if (dto.startTime==null)
        {
            dto.endTime=DateTime.MinValue;
        }
        
        return _temperatureDao.GetAsync(dto);
    }
}