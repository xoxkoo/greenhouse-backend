using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.Logic;

public class PresetLogic : IPresetLogic
{
    private readonly IPresetDao _presetDao;
    public PresetLogic(IPresetDao presetDao)
    {
        _presetDao = presetDao;
    }

    public async Task<IEnumerable<PresetDto>> GetAsync(SearchPresetParametersDto dto)
    {
        return await _presetDao.GetAsync(dto);
    }

    public async Task<PresetEfcDto> CreateAsync(PresetCreationDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Provided data cannot be null");
        }
        if (dto.Thresholds == null || dto.Thresholds.Count() != 3)
        {
            throw new ArgumentException("Exactly three thresholds must be provided", nameof(dto.Thresholds));
        }
        List<Threshold> thresholds = dto.Thresholds.Select(t => new Threshold
        {
            Type = t.Type,
            MaxValue = t.MaxValue,
            MinValue = t.MinValue
        }).ToList();

        foreach (var threshold in thresholds)
        {
            Console.WriteLine(threshold.Type);
            /*if (!threshold.Type.Equals("CO2") || !threshold.Type.Equals("Temperature") ||
                !threshold.Type.Equals("Humidity"))
            {
                throw new ArgumentException("Threshold type must be either: CO2, Humidity or Temperature.");
            }*/

            if (threshold.MaxValue < threshold.MinValue)
            {
                throw new ArgumentException("Min value cannot be bigger then max value.");
            }

            if (threshold.Type.Equals("CO2") && !(threshold.MinValue >= 0 && threshold.MaxValue <= 4095))
            {
                throw new ArgumentException("CO2 value is randing from 0 to 4095");
            }
            if (threshold.Type.Equals("Humidity") && !(threshold.MinValue >= 0 && threshold.MaxValue <= 100))
            {
                throw new ArgumentException("Humidity value is randing from 0 to 100");
            }
            if (threshold.Type.Equals("Temperature") && !(threshold.MinValue >= -50 && threshold.MaxValue <= 60))
            {
                throw new ArgumentException("Temperature value is randing from -50 to 60");
            }
        }

        Preset preset = new Preset
        {
            Name = dto.Name,
            Thresholds = thresholds,
            IsCurrent = false
        };

        return await _presetDao.CreateAsync(preset);
    }
}