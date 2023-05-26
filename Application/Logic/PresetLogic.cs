using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using SocketServer;

namespace Application.Logic;

public class PresetLogic : IPresetLogic
{
    private readonly IPresetDao _presetDao;
    private readonly IWebSocketServer _socketServer;
    private readonly IConverter _converter;

    public PresetLogic(IPresetDao presetDao, IWebSocketServer socketServer, IConverter converter)
    {
        _presetDao = presetDao;
        _socketServer = socketServer;
        _converter = converter;
    }

    public async Task<IEnumerable<PresetEfcDto>> GetAsync(SearchPresetParametersDto dto)
    {
        var presets = await _presetDao.GetAsync(dto);
        if (presets == null)
        {
            throw new Exception("Preset not found");
        }

        var result = new List<PresetEfcDto>();
        foreach (var p in presets)
        {
            var thresholds = new List<ThresholdDto>();
            if (p.Thresholds != null)
            {
                foreach (var t in p.Thresholds)
                {
                    var threshold = new ThresholdDto()
                    {
                        Max = t.Max,
                        Min = t.Min,
                        Type = t.Type
                    };
                    thresholds.Add(threshold);
                }
            }

            var presetToSend = new PresetEfcDto()
            {
                Id = p.Id,
                Name = p.Name,
                Thresholds = thresholds
            };
            result.Add(presetToSend);
        }

        return result;
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

    public async Task DeleteAsync(int id)
    {
        Preset? existing = await _presetDao.GetByIdAsync(id);
        if (existing == null)
        {
            throw new Exception($"Preset with ID {id} not found!");
        }
        if (existing.IsCurrent)
        {
            throw new Exception($"Preset with ID {id} is currently applied and therefore cannot be removed!");
        }
        await _presetDao.DeleteAsync(existing);
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

    public async Task ApplyAsync(int id)
    {
        //Find the preset which should be applied as a current and send to the IoT device
        PresetDto presetToSend = _presetDao.GetAsync(new SearchPresetParametersDto(id, null)).Result.FirstOrDefault();
        if (presetToSend == null)
        {
            throw new Exception($"Preset with id {id} not found");
        }
        //Change the value isCurrent to be true in database
        await _presetDao.ApplyAsync(id);
        string payload = _converter.ConvertPresetToHex(presetToSend);
        await _socketServer.Connect();
        await _socketServer.Send(payload);
        await _socketServer.Disconnect();
    }

    public async Task<PresetEfcDto> GetByIdAsync(int id)
    {
        SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(id, true);
        var preset = await _presetDao.GetByIdAsync(id);
        if (preset == null)
        {
            throw new Exception(
                $"Preset with id {id} was not found");
        }

        List<ThresholdDto> thresholdDtos = new List<ThresholdDto>();

        foreach (var t in preset.Thresholds)
        {
            ThresholdDto thresholdDto = new ThresholdDto()
            {
	            Type = t.Type,
                Max = t.MaxValue,
                Min = t.MinValue
            };
            thresholdDtos.Add(thresholdDto);
        }


        PresetEfcDto presetEfcDto = new PresetEfcDto()
        {
            Id = preset.Id,
            Name = preset.Name,
            Thresholds = thresholdDtos
        };
        return presetEfcDto;
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
