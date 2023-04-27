using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

namespace Application.LogicInterfaces;

public interface IConverter
{
    Task<string> ConvertFromHex(string payload);
    string ConvertActionsPayloadToHex(ValveStateDto dto, int duration);
}
