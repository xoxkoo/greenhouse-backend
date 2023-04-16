using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;

namespace Application.Logic;

public class HumidityLogic : IHumidityLogic
{
    private readonly IHumidityDao _humidityDao;

    public HumidityLogic(IHumidityDao humidityDao)
    {
        _humidityDao = humidityDao;
    }

    public Task<IEnumerable<HumidityDto>> GetAsync(SearchMeasurementDto dto)
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
        return _humidityDao.GetHumidityAsync(dto);
    }
}