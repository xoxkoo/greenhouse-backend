using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.LogicTests;

[TestClass]
public class ConverterTest : DbTestBase
{

	private readonly Mock<ITemperatureLogic> tempLogic;
	private readonly Mock<IHumidityLogic> humidityLogic;
	private readonly Mock<ICO2Logic> co2logic;
	private readonly Mock<IValveLogic> waterLogic;
	private readonly Mock<IEmailLogic> emailLogic;
	private readonly IConverter converter;


	public ConverterTest()
    {
	    tempLogic = new Mock<ITemperatureLogic>();
	    co2logic = new Mock<ICO2Logic>();
	    humidityLogic = new Mock<IHumidityLogic>();
	    waterLogic = new Mock<IValveLogic>();
	    emailLogic = new Mock<IEmailLogic>();
        converter = new Converter(tempLogic.Object, co2logic.Object, humidityLogic.Object, emailLogic.Object, waterLogic.Object);
    }

    [TestMethod]
    public async Task THCPayload_SavedToDB()
    {
	    await converter.ConvertFromHex("07817b0707f0");

        // Assert
        tempLogic.Verify(x => x.CreateAsync(It.Is<TemperatureCreateDto>(dto =>
	        // tolerance because of rounding problems
	        Math.Abs(dto.Value - 25.8) < 0.1
        )), Times.Once);

        co2logic.Verify(x => x.CreateAsync(It.Is<CO2CreateDto>(dto =>
	        dto.Value == 2032
        )), Times.Once);

        humidityLogic.Verify(x => x.CreateAsync(It.Is<HumidityCreationDto>(dto =>
	        dto.Value == 5
        )), Times.Once);
    }

    [TestMethod]
    public async Task THCPayload_IncorrectMeasurementSensor()
    {
	    await converter.ConvertFromHex("04047b0707f0");

        // Assert
        // values shouldn't be saved
        tempLogic.Verify(x => x.CreateAsync(It.Is<TemperatureCreateDto>(dto =>
	        // tolerance because of rounding problems
	        Math.Abs(dto.Value - 25.8) < 0.1
        )), Times.Never);

        co2logic.Verify(x => x.CreateAsync(It.Is<CO2CreateDto>(dto =>
	        dto.Value == 2032
        )), Times.Never);

        humidityLogic.Verify(x => x.CreateAsync(It.Is<HumidityCreationDto>(dto =>
	        dto.Value == 56
        )), Times.Never);
    }

    [TestMethod]
    public async Task THCPayload_ResponseStringIsCorrect()
    {
	    string result = await converter.ConvertFromHex("07817b0707f0");

	    Assert.AreEqual("25.8, 5, 2032", result);
    }

    [TestMethod]
    public void THCPayload_ThrowErrorWhenNotHexValue()
    {
	    Assert.ThrowsExceptionAsync<Exception>(() => converter.ConvertFromHex("t7800c9401e0"));
    }

    [TestMethod]
    public async Task THCPayload_IncorrectValue()
    {
	    Assert.ThrowsExceptionAsync<Exception>(() => converter.ConvertFromHex(""));
	    Assert.ThrowsExceptionAsync<Exception>(() => converter.ConvertFromHex("    "));
    }

    [TestMethod]
    public void IntervalPayload_DataAreSent()
    {
	    var intervals = GetIntervals(7);


	    string result = converter.ConvertIntervalToHex(intervals);
	    string expected = "09cf04073c101cf04073c101cf04073c101cf040";

	    Assert.AreEqual(result, expected);
    }

    [TestMethod]
    public void IntervalPayload_MaximumAmountOfIntervalsReached()
    {
	    // 7 intervals is maximum amount
	    var intervals = GetIntervals(12);

	    Assert.ThrowsException<Exception>(() =>
		    converter.ConvertIntervalToHex(intervals));
    }

    [TestMethod]
    public void IntervalPayload_HoursAndMinutesAreMinBoundaries()
    {
	    var intervals = new List<IntervalToSendDto>();
	    var interval = new IntervalToSendDto()
		    { StartTime = TimeSpan.FromHours(0) + TimeSpan.FromMinutes(0), EndTime = TimeSpan.FromHours(0) + TimeSpan.FromMinutes(0) };
	    intervals.Add(interval);

	    string result = converter.ConvertIntervalToHex(intervals);

	    Assert.AreEqual(result, "080000");
    }

    [TestMethod]
    public void IntervalPayload_HoursAndMinutesAreMaxBoundaries()
    {
	    var intervals = new List<IntervalToSendDto>();
	    var interval = new IntervalToSendDto()
		    { StartTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59), EndTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59) };
	    intervals.Add(interval);

	    string result = converter.ConvertIntervalToHex(intervals);

	    Assert.AreEqual(result, "0afddfb0");
    }


    private IEnumerable<IntervalToSendDto> GetIntervals(int n)
    {
	    var intervals = new List<IntervalToSendDto>();
	    for (int i = 0; i < n; i++)
	    {
		    var interval = new IntervalToSendDto()
			    {StartTime = TimeSpan.FromHours(14) + TimeSpan.FromMinutes(30), EndTime = TimeSpan.FromHours(1) + TimeSpan.FromMinutes(0) };
		    intervals.Add(interval);

	    }
	    return intervals;
    }

    [TestMethod]
    public async Task ActionsPayload_CorrectStringResponse()
    {
	    ValveStateCreationDto dto = new()
	    {
		    State = true,
		    duration = 16
	    };
	    string result = converter.ConvertActionsPayloadToHex(dto);
	    Assert.AreEqual("160010", result);
    }
    [TestMethod]
    public async Task ActionsPayload_DurationOverLimit()
    {
	    ValveStateCreationDto dto = new()
	    {
		    State = true,
		    duration = 1024
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto));

    }
    [TestMethod]
    public async Task ActionsPayload_DurationTooLow()
    {
	    ValveStateCreationDto dto = new()
	    {
		    State = true,
		    duration = -1
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto));
    }
    [TestMethod]
    public void ActionsPayload_NullDto()
    {
        Assert.ThrowsException<NullReferenceException>(() => converter.ConvertActionsPayloadToHex(null));
    }

    [TestMethod]
    public void ActionsPayload_ToggleFalseCorrectDuration()
    {
	    ValveStateCreationDto dto = new()
	    {
		    State = false,
		    duration = 1
	    };
        Assert.AreEqual("140001", converter.ConvertActionsPayloadToHex(dto));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueCorrectDuration()
    {
	    ValveStateCreationDto dto = new()
	    {
		    State = true,
		    duration = 1
	    };
	    Assert.AreEqual("160001", converter.ConvertActionsPayloadToHex(dto));
    }
    [TestMethod]
    public void ActionsPayload_ToggleFalseIncorrectDuration()
    {
	    ValveStateCreationDto dto = new()
	    {
		    State = false,
		    duration = 1024
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueIncorrectDuration()
    {
	    ValveStateCreationDto dto = new()
	    {
		    State = true,
		    duration = 100000
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueIncorrectNegativeDuration()
    {
	   ValveStateCreationDto dto = new()
	    {
		    State = true,
		    duration = -1
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto));
    }

		[TestMethod]
	    public void ConvertPresetToHex_NullPresetDto()
	    {
		    // Arrange
		    PresetDto presetDto = null;

		    // Act & Assert
		    Assert.ThrowsException<NullReferenceException>(() => converter.ConvertPresetToHex(presetDto));
	    }

	    [TestMethod]
	    public void ConvertPresetToHex_MissingThresholds()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto { Id = 3 };

		    // Act & Assert
		    Assert.ThrowsException<ArgumentNullException>(() => converter.ConvertPresetToHex(presetDto));
	    }

	    [TestMethod]
	    public void ConvertPresetToHex_EmptyThresholds()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto { Id = 3, Thresholds = new List<ThresholdDto>() };

		    // Act & Assert
		    Assert.ThrowsException<Exception>(() => converter.ConvertPresetToHex(presetDto));

	    }

	    [TestMethod]
	    public void ConvertPresetToHex_ValidPresetDto()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    //000011
			    Id = 3,
			    Thresholds = new List<ThresholdDto>
			    {
				    //22 bits
				    //20*10+500 = 010 10111100 30*10+500 = 011 0010 0000
				    new ThresholdDto { Type = "temperature", Min = 20, Max = 30 },
				    //14 bits
				    //40 = 0101000 60 = 0111100
				    new ThresholdDto { Type = "humidity", Min = 40, Max = 60 },
				    //24 bits
				    //100 = 000001100100 200 = 000011001000
				    new ThresholdDto { Type = "co2", Min = 100, Max = 200 }
			    }
		    };

		    // Act
		    //00001101 01011110 00110010 00000101 00001111 00000001 10010000 00110010 00
		    // 0d        5e        32       05       0f        01     90       32     00
		    string resultHex = converter.ConvertPresetToHex(presetDto);

		    // Assert
		    Assert.AreEqual("115e32050f00c80c80", resultHex);
	    }



	    [TestMethod]
	    public void ConvertPresetToHex_ThresholdWithInvalidType()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    //000011
			    Id = 3,
			    Thresholds = new List<ThresholdDto>
			    {
				    //010 10111100011 0010 0000
				    new ThresholdDto { Type = "temperature", Min = 20, Max = 30 },
				    //0000 0000 0000 00
				    new ThresholdDto { Type = "unknown", Min = 40, Max = 60 },
				    //000001 10010000 00110010 00
				    new ThresholdDto { Type = "co2", Min = 100, Max = 200 }
			    }
		    };

		    // Act & Assert
		    Assert.ThrowsException<Exception>(() => converter.ConvertPresetToHex(presetDto));
	    }

	    [TestMethod]
	    public void ConvertPresetToHex_ThresholdsWithNegativeValues()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    Id = 3,
			    Thresholds = new List<ThresholdDto>
			    {
				    // 22bits
				    //00110010000 011 0010 0000
				    new ThresholdDto { Type = "temperature", Min = -10, Max = 20 },
				    //14 bits
				    //40 = 0101000 60 = 0111100
				    new ThresholdDto { Type = "humidity", Min = 40, Max = 60 },
				    //24 bits
				    //100 = 000001100100 200 = 000011001000
				    new ThresholdDto { Type = "co2", Min = 100, Max = 200 }
			    }
		    };

		    // Act
		    string resultHex = converter.ConvertPresetToHex(presetDto);

		    // Assert
		    Assert.AreEqual("10c82bc50f00c80c80", resultHex);
	    }

	    [TestMethod]
	    public void ConvertPresetToHex_ThresholdsWithBoundaryValues()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    Id = 3,
			    Thresholds = new List<ThresholdDto>
			    {
				    // 22bits
				    //00 11001000 00110010 0000
				    new ThresholdDto { Type = "temperature", Min = -50, Max = 60 },
				    //14 bits
				    //40 = 0101000 60 = 0111100
				    new ThresholdDto { Type = "humidity", Min = 0, Max = 100 },
				    //24 bits
				    //100 = 000001100100 200 = 000011001000
				    new ThresholdDto { Type = "co2", Min = 0, Max = 4095 }
			    }
		    };

		    // Act
		    string resultHex = converter.ConvertPresetToHex(presetDto);

		    // Assert
		    Assert.AreEqual("100044c0190000fff0", resultHex);
	    }


	    [TestMethod]
	    public void ConvertPresetToHex_ThresholdsValueOutOfRange()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    Id = 3,
			    Thresholds = new List<ThresholdDto>
			    {
				    // 22bits
				    //00 11001000 00110010 0000
				    new ThresholdDto { Type = "temperature", Min = -50, Max = 60 },
				    //14 bits
				    //40 = 0101000 60 = 0111100
				    new ThresholdDto { Type = "humidity", Min = 0, Max = 101 },
				    //24 bits
				    //100 = 000001100100 200 = 000011001000
				    new ThresholdDto { Type = "co2", Min = 0, Max = 4096 }
			    }
		    };

		    // Act
		    Assert.ThrowsException<ArgumentOutOfRangeException>(() => converter.ConvertPresetToHex(presetDto));
	    }
}
