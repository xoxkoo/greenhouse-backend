using System.Text;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.DTOs.PayloadDTOs;

namespace Application.Logic;

public class Converter : IConverter
{
    private static readonly Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string> {
        { '0', "0000" },
        { '1', "0001" },
        { '2', "0010" },
        { '3', "0011" },
        { '4', "0100" },
        { '5', "0101" },
        { '6', "0110" },
        { '7', "0111" },
        { '8', "1000" },
        { '9', "1001" },
        { 'a', "1010" },
        { 'b', "1011" },
        { 'c', "1100" },
        { 'd', "1101" },
        { 'e', "1110" },
        { 'f', "1111" }
    };


    private ITemperatureLogic temperatureLogic;
    private ICO2Logic co2Logic;
    private IHumidityLogic humidityLogic;

    public Converter(ITemperatureLogic temperatureLogic, ICO2Logic co2Logic, IHumidityLogic humidityLogic)
    {
        this.temperatureLogic = temperatureLogic;
        this.co2Logic = co2Logic;
        this.humidityLogic = humidityLogic;
    }

    public async Task ConvertFromHex(string payload)
    {
        string valueInBinary = HexStringToBinary(payload);
        Console.WriteLine(valueInBinary);
        //000001111000000000001100100101000000000111100000
        string id = valueInBinary.Substring(0, 6);
        switch (id)
        {
            case "000001":
                ReadTHCPayload(valueInBinary.Substring(6));
                break;
            //TODO 
            //handle rest of payloads
        }
    }

    private async void ReadTHCPayload(string data)
    {
        Console.WriteLine(data);
        //TODO handle flags
        string flags = data.Substring(0, 8);
        string temperature = data.Substring(8, 11);
        string humidity = data.Substring(19, 7);
        string co2 = data.Substring(26, 12);
        
        TemperatureCreateDto tempDto = new TemperatureCreateDto()
        {
            Date = DateTime.Now,
            value = Convert.ToInt32(temperature, 2)
        };

        CO2CreateDto co2Dto = new CO2CreateDto
        {
            Date = DateTime.Now,
            Value = Convert.ToInt32(co2, 2)
        };
        HumidityCreationDto humidityDto = new HumidityCreationDto
        {
            Date = DateTime.Now,
            Value = Convert.ToInt32(humidity, 2)
        };

        await temperatureLogic.CreateAsync(tempDto);
        await co2Logic.CreateAsync(co2Dto);
        await humidityLogic.CreateAsync(humidityDto);
    }

    public string HexStringToBinary(string hex) {
        StringBuilder result = new StringBuilder();
        foreach (char c in hex) {
            //TODO 
            //Check for exceptions
            // This will crash for non-hex characters. You might want to handle that differently.
            result.Append(hexCharacterToBinary[char.ToLower(c)]);
        }
        return result.ToString();
    }
    
    
}