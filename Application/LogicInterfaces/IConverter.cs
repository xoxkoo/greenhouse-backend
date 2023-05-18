using Domain.DTOs;

namespace Application.LogicInterfaces;

public interface IConverter
{
    Task<string> ConvertFromHex(string payload);
    string ConvertIntervalToHex(IEnumerable<IntervalToSendDto> intervals);
    string ConvertActionsPayloadToHex(ValveStateDto dto, int duration);
    string ConvertPresetToHex(PresetDto dto);
}
