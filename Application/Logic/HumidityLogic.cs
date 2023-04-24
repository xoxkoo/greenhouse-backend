using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.Logic;

public class HumidityLogic : IHumidityLogic
{
    private readonly IHumidityDao _humidityDao;

    public HumidityLogic(IHumidityDao humidityDao)
    {
        _humidityDao = humidityDao;
    }
    
    public async Task<HumidityDto> CreateAsync(HumidityCreationDto dto)
    {
        //TODO add validation

        var entity = new Humidity()
        {
            Date = dto.Date,
            Value = dto.Value
        };

        return await _humidityDao.CreateAsync(entity);
    }

    public async Task<IEnumerable<HumidityDto>> GetAsync(SearchMeasurementDto dto)
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

        return await _humidityDao.GetHumidityAsync(dto);
    }


}
