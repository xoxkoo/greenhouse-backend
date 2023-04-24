using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;


namespace Application.Logic;

public class TemperatureLogic : ITemperatureLogic
{
    private readonly ITemperatureDao _temperatureDao;
    private IConverter converter;
    public TemperatureLogic(ITemperatureDao temperatureDao)
    {
        _temperatureDao = temperatureDao;
    }

    public async Task<TemperatureDto> CreateAsync(TemperatureCreateDto dto)
    {

        Temperature temperature = new Temperature
        {
            Date = DateTime.Now,
            Value = dto.value
        };
        return await _temperatureDao.CreateAsync(temperature);
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
