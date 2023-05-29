using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

namespace Application.LogicInterfaces;

public interface IConverter
{
    Task<string> ConvertFromHex(string payload);
    string ConvertIntervalToHex(IEnumerable<IntervalToSendDto> intervals, bool clear = false);
    string ConvertActionsPayloadToHex(ValveStateCreationDto dto);
    string ConvertPresetToHex(PresetDto dto);
}
