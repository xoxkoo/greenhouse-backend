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
        ValidateInput(dto);

        List<Threshold> thresholds = MapThresholds(dto.Thresholds);
        ValidateThresholds(thresholds);

        Preset preset = new Preset
        {
            Name = dto.Name,
            Thresholds = thresholds,
            IsCurrent = false
        };

        return await _presetDao.CreateAsync(preset);
    }

    public async Task<PresetEfcDto> UpdateAsync(PresetEfcDto dto)
    {
	    if (dto == null)
	    {
		    throw new ArgumentNullException(nameof(dto), "Provided data cannot be null");
	    }
	    if (dto.Thresholds == null || dto.Thresholds.Count() != 3)
	    {
		    throw new ArgumentException("Exactly three thresholds must be provided");
	    }

	    List<Threshold> thresholds = MapThresholds(dto.Thresholds);
	    ValidateThresholds(thresholds);

	    return await _presetDao.UpdateAsync(dto);
    }

    private void ValidateInput(PresetCreationDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Provided data cannot be null");
        }
        if (dto.Thresholds == null || dto.Thresholds.Count() != 3)
        {
            throw new ArgumentException("Exactly three thresholds must be provided");
        }
    }

    private List<Threshold> MapThresholds(IEnumerable<ThresholdDto> thresholdDtos)
    {
        List<Threshold> thresholds = thresholdDtos.Select(t => new Threshold
        {
            Type = t.Type,
            MaxValue = t.Max,
            MinValue = t.Min
        }).ToList();

        if (HasDuplicateThresholdTypes(thresholds))
        {
            throw new ArgumentException("There must be exactly three thresholds named: CO2, Humidity, and Temperature.");
        }

        return thresholds;
    }

    private bool HasDuplicateThresholdTypes(List<Threshold> thresholds)
    {
        var uniqueTypes = new HashSet<string>();
        foreach (var threshold in thresholds)
        {
            if (!uniqueTypes.Add(threshold.Type.ToLower()))
            {
                return true;
            }
        }
        return false;
    }

    private void ValidateThresholds(List<Threshold> thresholds)
    {
        foreach (var threshold in thresholds)
        {
            NormalizeThresholdType(threshold);

            if (threshold.MaxValue < threshold.MinValue)
            {
                throw new ArgumentException("Min value cannot be bigger than max value.");
            }

            if (!IsValidThresholdRange(threshold))
            {
                throw new ArgumentException(GetThresholdRangeErrorMessage(threshold));
            }
        }
    }

    private void NormalizeThresholdType(Threshold threshold)
    {
        if (threshold.Type.ToLower() == "co2")
        {
            threshold.Type = "CO2";
        }
        else
        {
            threshold.Type = char.ToUpperInvariant(threshold.Type[0]) + threshold.Type.Substring(1).ToLowerInvariant();
        }
    }

    private bool IsValidThresholdRange(Threshold threshold)
    {
        switch (threshold.Type)
        {
            case "CO2":
                return threshold.MinValue >= 0 && threshold.MaxValue <= 4095;
            case "Humidity":
                return threshold.MinValue >= 0 && threshold.MaxValue <= 100;
            case "Temperature":
                return threshold.MinValue >= -50 && threshold.MaxValue <= 60;
            default:
                return false;
        }
    }

    private string GetThresholdRangeErrorMessage(Threshold threshold)
    {
        switch (threshold.Type)
        {
            case "CO2":
                return "CO2 value must range from 0 to 4095.";
            case "Humidity":
                return "Humidity value must range from 0 to 100.";
            case "Temperature":
                return "Temperature value must range from -50 to 60.";
            default:
                return "Invalid threshold type.";
        }
    }
}
