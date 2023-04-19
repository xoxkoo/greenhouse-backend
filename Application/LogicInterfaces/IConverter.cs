namespace Application.LogicInterfaces;

public interface IConverter
{
    Task ConvertFromHex(string payload);
}