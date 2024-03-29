﻿using System.Text;
using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

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
    private IValveLogic valveLogic;

    public Converter(ITemperatureLogic temperatureLogic, ICO2Logic co2Logic, IHumidityLogic humidityLogic, IEmailLogic emailLogic, IValveLogic valveLogic)
    {
        this.temperatureLogic = temperatureLogic;
        this.co2Logic = co2Logic;
        this.humidityLogic = humidityLogic;
        this.emailLogic = emailLogic;
        this.valveLogic = valveLogic;
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
     * Convert actions payload into hexadecimal payload
     *
     * @param actionsPayload
     */
    public string ConvertActionsPayloadToHex(ValveStateCreationDto dto)
    {
        //  ID 6 bits
        //  Actions 8bits - 7th bit water toggle
        // Interval 10bits - 1023 minutes
        StringBuilder result = new StringBuilder();

        //ID for this payload is 5
        result.Append("000101");

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
        if (dto.duration < 0 || dto.duration > 1023)
        {
            throw new Exception("Duration must be an integer between 0 and 1023.");
        }
        // Interval in minutes, total size of 10 bits
        result.Append(IntToBinaryLeft(dto.duration, 10));

        // Return a hex representation of provided binary payload
        return BinaryStringToHex(result.ToString()).ToLower();
    }
    /**
     * Convert schedule into hexadecimal payload
     *
     * @param intervals
     */
    public string ConvertIntervalToHex(IEnumerable<IntervalToSendDto> intervals, bool clear = false)
    {
	    // max allowed count is 7
	    if (intervals.Count() > 7)
	    {
		    throw new Exception("Too many intervals");
	    }

	    // set the ide to be 2 or 3, depending if we want to clear intervals
	    // (2 -> 10 in binary) - clear intervals
	    // (3 -> 11 in binary) - append intervals
	    string payloadBinary = (clear) ? "11" : "10";

	    // loop through the intervals and convert
	    foreach (var interval in intervals)
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
	    string payloadHex = '0' + BinaryStringToHex(payloadBinary).ToLower();

	    // hex value must be even
	    if (payloadHex.Length % 2 == 0)
		    return payloadHex;
	    if (payloadHex[^1] == '0')
		    return payloadHex.Remove(payloadHex.Length - 1);

	    return payloadHex + '0';
    }
    public string ConvertPresetToHex(PresetDto dto)
    {
	    StringBuilder result = new StringBuilder();

	    //ID - 6 bits
	    //ID for this payload is 4
	    result.Append("000100");
	    List<ThresholdDto> thresholds = dto.Thresholds.ToList();
	    if (thresholds == null)
	    {
		    throw new NullReferenceException("Thresholds cannot be null");
	    }

	    if (thresholds.Count() != 3)
	    {
		    throw new Exception("In the preset there have to be three thresholds");
	    }

	    foreach (var t in thresholds)
	    {
		    if (t.Type.ToLower() != "temperature" && t.Type.ToLower() != "co2" && t.Type.ToLower() != "humidity")		    {
			    throw new Exception("In the preset the types of the thresholds have to be: temperature, co2 or humidity");
		    }
	    }

	    //Temperature range - 22 bits
	    ThresholdDto temperatureThreshold = thresholds.FirstOrDefault(t => t.Type.ToLower().Equals("temperature"));
	    if (temperatureThreshold == null)
	    {
		    throw new Exception($"Thresholds for temperature was not found for {dto.Id}");
	    }
	    if (temperatureThreshold.Min < -50 || temperatureThreshold.Max > 60)
	    {
			    throw new ArgumentOutOfRangeException(temperatureThreshold.Type, "The value of the temperature is out of range -50 to 60");
	    }
	    result.Append(IntToBinaryLeft((int)temperatureThreshold.Min*10 + 500, 11));
	    result.Append(IntToBinaryLeft((int)temperatureThreshold.Max*10 + 500, 11));

	    //Humidity range - 14 bits
	    ThresholdDto humidityThreshold = thresholds.FirstOrDefault(t => t.Type.ToLower().Equals("humidity"));
	    if (humidityThreshold == null)
	    {
		    throw new Exception($"Thresholds for humidity was not found for {dto.Id}");
	    }
	    if (humidityThreshold.Min < 0 || humidityThreshold.Max > 100)
	    {
		    throw new ArgumentOutOfRangeException(humidityThreshold.Type, "The value of the humidity is out of range 0 to 100");
	    }
	    result.Append(IntToBinaryLeft((int)humidityThreshold.Min, 7));
	    result.Append(IntToBinaryLeft((int)humidityThreshold.Max, 7));



	    //CO2 range - 24 bits
	    ThresholdDto co2Threshold = thresholds.FirstOrDefault(t => t.Type.ToLower().Equals("co2"));
	    if (co2Threshold == null)
	    {
		    throw new Exception($"Thresholds for co2 was not found for {dto.Id}");
	    }
	    if (co2Threshold.Min < 0 || co2Threshold.Max > 4095)
	    {
		    throw new ArgumentOutOfRangeException( co2Threshold.Type,"The value of the co2 is out of range 0 to 4095");
	    }

	    result.Append(IntToBinaryLeft((int)co2Threshold.Min, 13));
	    result.Append(IntToBinaryLeft((int)co2Threshold.Max, 13));

	    return BinaryStringToHex(result.ToString()).ToLower();
    }


    private async Task<string> ReadTHCPayload(string data)
    {
	    string flags = data.Substring(0, 8);
        string temperature = data.Substring(8, 11);
        string humidity = data.Substring(19, 10);
        string co2 = data.Substring(29, 13);

        float tmpValue = ((float)Convert.ToInt32(temperature, 2)) / 10 - 50;
        int humValue = (Convert.ToInt32(humidity, 2)) / 10;
        int co2Value = Convert.ToInt32(co2, 2);

        TemperatureCreateDto tempDto = new TemperatureCreateDto()
        {
            Date = DateTime.Now,
            Value = (float)MathF.Round(tmpValue, 1)
        };

        CO2CreateDto co2Dto = new CO2CreateDto
        {
            Date = DateTime.Now,
            Value = co2Value
        };
        HumidityCreationDto humidityDto = new HumidityCreationDto
        {
            Date = DateTime.Now,
            Value = humValue
        };


        // check if sensors measurements are valid
        if (Int32.Parse(flags.Substring(0,1)) == 1)
	        await temperatureLogic.CreateAsync(tempDto);

        if (Int32.Parse(flags.Substring(1,1)) == 1)
			await humidityLogic.CreateAsync(humidityDto);

        if (Int32.Parse(flags.Substring(2,1)) == 1)
			await co2Logic.CreateAsync(co2Dto);

        // check for valve state
        if (Int32.Parse(flags.Substring(7, 1)) == 1)
	        await valveLogic.SetAsync(new ValveStateCreationDto(){State = true});
        else
	        await valveLogic.SetAsync(new ValveStateCreationDto(){State = false});

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
