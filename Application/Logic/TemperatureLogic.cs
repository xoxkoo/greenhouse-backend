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
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Temperature object cannot be null");
        }
        if (dto.Value < -50)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Value), "Value of temperature cannot be below -50°C");
        }
        if (dto.Value > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Value), "Value of temperature cannot be above 60°C");
        }
        if (dto.Date > DateTime.Now)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Date), "Date of temperature cannot be in the future");
        }
        Temperature temperature = new Temperature
        {
            Date = DateTime.Now,
            Value = dto.Value
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
            dto.StartTime=DateTime.MinValue;
        }

        return await _temperatureDao.GetAsync(dto);
    }


}
