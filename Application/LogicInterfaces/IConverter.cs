using Domain.DTOs;

namespace Application.LogicInterfaces;

public interface IConverter
{
    Task<string> ConvertFromHex(string payload);
    string ConvertIntervalToHex(ScheduleDto intervals);
}
