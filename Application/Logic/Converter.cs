using System.Text;
using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;

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
    private IEmailLogic emailLogic;
    public Converter(ITemperatureLogic temperatureLogic, ICO2Logic co2Logic, IHumidityLogic humidityLogic, IEmailLogic emailLogic)
    {
        this.temperatureLogic = temperatureLogic;
        this.co2Logic = co2Logic;
        this.humidityLogic = humidityLogic;
        this.emailLogic = emailLogic;
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
        }

        return response;
    }
    /**
     * Convert actions payload into hexidecimal payload
     *
     * @param actionsPayload
     */
    public string ConvertActionsPayloadToHex(ValveStateDto dto, int duration)
    {
        //  ID 6 bits
        //  Actions 8bits - 7th bit water toggle
        // Interval 10bits - 1023 minutes
        StringBuilder result = new StringBuilder();

        //ID for this payload is 4
        result.Append("000100");

        int toggleBit = 0;
        // bit is 1 if toggle is true, 0 if false
        if (dto.State)
        {
            toggleBit = 1;
        }

        // Validation for toggle bit
        if ((dto.State && toggleBit != 1) || (!dto.State && toggleBit != 0))
        {
            throw new Exception("State bit error.");
        }
        // 7th bit is either 0 or 1, total size 8 bits
        result.Append(IntToBinaryRight(toggleBit, 8));

        // Validation for duration
        if (duration < 0 || duration > 1023)
        {
            throw new Exception("Duration must be an integer between 0 and 1023.");
        }
        // Interval in minutes, total size of 10 bits
        result.Append(IntToBinaryLeft(duration, 10));

        // Return a hex representation of provided binary payload
        return BinaryStringToHex(result.ToString()).ToLower();
    }
    /**
     * Convert schedule into hexadecimal payload
     *
     * @param intervals
     */
    public string ConvertIntervalToHex(ScheduleToSendDto intervals)
    {
	    // max allowed count is 7
	    if (intervals.Intervals.Count() > 7)
	    {
		    throw new Exception("Too many intervals");
	    }

	    // set the ide to be 2 (2 -> 10 in binary)
	    string payloadBinary = "10";


	    // loop through the intervals and convert
	    foreach (var interval in intervals.Intervals)
	    {

		    int startHours = interval.StartTime.Hours;
		    int startMinutes = interval.StartTime.Minutes;

		    int endHours = interval.EndTime.Hours;
		    int endMinutes = interval.EndTime.Minutes;

		    // convert start and end hours and minutes to hexadecimal
		    // hours should be 5 bits, minutes 6 bits
		    payloadBinary += IntToBinary(startHours, 5);
		    payloadBinary += IntToBinary(startMinutes, 6);
		    payloadBinary += IntToBinary(endHours, 5);
		    payloadBinary += IntToBinary(endMinutes, 6);


	    }

	    // return the hex value of payload
	    // prepend 0 in the beginning, because provided binary string length must be a Dividable of 8
	    return '0' + BinaryStringToHex(payloadBinary).ToLower();
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
        await emailLogic.CheckIfInRange(tempDto.Value, humidityDto.Value, co2Dto.Value);
        return $"{tempDto.Value}, {humidityDto.Value}, {co2Dto.Value}";
    }

    private string BinaryStringToHex(string binary)
    {
       if (string.IsNullOrEmpty(binary))
       {
          throw new ArgumentException("The binary string cannot be null or empty.");
       }

       binary = PadToMultipleOf8(binary);

       if (binary.Length % 8 != 0)
       {
          throw new ArgumentException("The binary string length must be a multiple of 8.");
       }

       byte[] binaryData = new byte[binary.Length / 8];
       for (int i = 0; i < binaryData.Length; i++)
       {
          binaryData[i] = Convert.ToByte(binary.Substring(i * 8, 8), 2);
       }

       return BitConverter.ToString(binaryData).Replace("-", "");
    }

    private string IntToBinaryLeft(int number, int totalSize)
    {
       string binary = Convert.ToString(number, 2);

       // prepend a zero to the binary string so the total length of the binary string is totalSize
       return binary.PadLeft((totalSize - binary.Length) + binary.Length, '0');

    }
    private string IntToBinaryRight(int number, int totalSize)
    {
        string binary = Convert.ToString(number, 2);

        // prepend a zero to the binary string so the total length of the binary string is totalSize
        return binary.PadRight((totalSize - binary.Length) + binary.Length, '0');

    }

    /**
     * Appends 0s to the end of a string to make its length divisible by 8.
     */
    private string PadToMultipleOf8(string value)
    {
	    const int multiple = 8; // Define the multiple we want to pad to.

	    int currentLength = value.Length;
	    int desiredLength = (currentLength + multiple - 1) / multiple * multiple; // Round up to nearest multiple of 8.

	    return value.PadRight(desiredLength, '0');
    }

    private string IntToBinary(int number, int totalSize)
    {
	    string binary = Convert.ToString(number, 2);

	    // prepend a zero to the binary string so the total length of the binary string is totalSize
	    return binary.PadLeft((totalSize - binary.Length) + binary.Length, '0');

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
