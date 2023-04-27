using System.Text;
using Application.DaoInterfaces;
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
    private IWateringSystemLogic wateringSystemLogic;
    public Converter(ITemperatureLogic temperatureLogic, ICO2Logic co2Logic, IHumidityLogic humidityLogic, IWateringSystemLogic wateringSystemLogic)
    {
        this.temperatureLogic = temperatureLogic;
        this.co2Logic = co2Logic;
        this.humidityLogic = humidityLogic;
        this.wateringSystemLogic = wateringSystemLogic;
    }

    public async Task<string> ConvertFromHex(string payload)
    {
        string valueInBinary = HexStringToBinary(payload);
        string response = "";

        //000001 111000000000001100100101000000000111100000
        string id = valueInBinary.Substring(0, 6);
        switch (id)
        {
            case "000001":
                response = await ReadTHCPayload(valueInBinary.Substring(6));
                break;
            //TODO
            //handle rest of payloads
        }

        return response;
    }

    public async Task<string> ActionsPayload(ValveStateDto dto, int duration)
    {
        //  ID 6 bits
        //  Actions 8bits - 7th bit water toggle
        // Interval 10bits - 1023 minutes
        StringBuilder result = new StringBuilder();
        
        int toogleBit = 0;
        if (dto.Toggle)
        {
            toogleBit = 1;
        }
        
        //ID for this payload is 4
        result.Append(4);
        
        // 7th bit for water - either 0 or 1
        if ((dto.Toggle && toogleBit != 1) || (!dto.Toggle && toogleBit != 0))
        {
            throw new Exception("Toggle bit error.");
        }
        result.Append(toogleBit);
        
        //validation for duration
        if (duration < 0 || duration > 1023)
        {
            throw new Exception("Duration must be an integer between 0 and 1023.");
        }
        //Interval in minutes
        result.Append(Convert.ToString(duration, 16));
        
        return result.ToString();
    }
    private async Task<string> ReadTHCPayload(string data)
    {
        //TODO handle flags
        string flags = data.Substring(0, 8);
        string temperature = data.Substring(8, 11);
        string humidity = data.Substring(19, 7);
        string co2 = data.Substring(26, 12);

        float tmpValue = ((float)Convert.ToInt32(temperature, 2)) / 10 - 50;

        TemperatureCreateDto tempDto = new TemperatureCreateDto()
        {
            Date = DateTime.Now,
            Value = (float)MathF.Round(tmpValue, 1)
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

        await co2Logic.CreateAsync(co2Dto);
        await humidityLogic.CreateAsync(humidityDto);
        await temperatureLogic.CreateAsync(tempDto);

        return $"{tempDto.Value}, {humidityDto.Value}, {co2Dto.Value}";
    }

    private string HexStringToBinary(string hex) {
        StringBuilder result = new StringBuilder();

        if (hex is not string)
        {
	        throw new Exception("Hex value must be a string!");
        }

        if (hex.Trim().Length == 0)
        {
			throw new Exception("Empty value is not allowed!");
        }

        foreach (char c in hex) {

            if (! hexCharacterToBinary.ContainsKey(c))
            {
	            throw new Exception($"Invalid character in hex value: {c}");
            }

	        result.Append(hexCharacterToBinary[char.ToLower(c)]);
        }
        return result.ToString();
    }


}
