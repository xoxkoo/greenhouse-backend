using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;


namespace Application.Logic;

public class TemperatureLogic : ITemperatureLogic
{
    private readonly ITemperatureDao _temperatureDao;

    public TemperatureLogic(ITemperatureDao temperatureDao)
    {
        _temperatureDao = temperatureDao;
    }

    public async Task<IEnumerable<TemperatureDto>> GetAsync(SearchMeasurementDto dto)
    {
        if (dto.StartTime>dto.EndTime)
        {
            throw new Exception("Start date cannot be before the end date");
        }

        if (dto.EndTime==null)
        {
            dto.EndTime=DateTime.MaxValue;
        }
        if (dto.StartTime==null)
        {
            dto.EndTime=DateTime.MinValue;
        }

        return await _temperatureDao.GetAsync(dto);
    }
}
