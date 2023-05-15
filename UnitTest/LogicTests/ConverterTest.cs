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
	private readonly Mock<IWateringSystemLogic> waterLogic;
	private readonly Mock<IEmailLogic> emailLogic;
	private readonly IConverter converter;


    public ConverterTest()
    {
	    tempLogic = new Mock<ITemperatureLogic>();
	    co2logic = new Mock<ICO2Logic>();
	    humidityLogic = new Mock<IHumidityLogic>();
	    waterLogic = new Mock<IWateringSystemLogic>();
	    emailLogic = new Mock<IEmailLogic>();
        converter = new Converter(tempLogic.Object, co2logic.Object, humidityLogic.Object, emailLogic.Object);
    }

    [TestMethod]
    public async Task THCPayload_SavedToDB()
    {
	    await converter.ConvertFromHex("07817b1f4ff0");

        // Assert
        tempLogic.Verify(x => x.CreateAsync(It.Is<TemperatureCreateDto>(dto =>
	        // tolerance because of rounding problems
	        Math.Abs(dto.Value - 25.8) < 0.1
        )), Times.Once);

        co2logic.Verify(x => x.CreateAsync(It.Is<CO2CreateDto>(dto =>
	        dto.Value == 1279
        )), Times.Once);

        humidityLogic.Verify(x => x.CreateAsync(It.Is<HumidityCreationDto>(dto =>
	        dto.Value == 31
        )), Times.Once);
    }

    [TestMethod]
    public async Task THCPayload_ResponseStringIsCorrect()
    {
	    string result = await converter.ConvertFromHex("07817b1f4ff0");

	    Assert.AreEqual("25.8, 31, 1279", result);
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


	    string result = converter.ConvertIntervalToHex(new ScheduleToSendDto(){Intervals = intervals});
	    string expected = "09cf04073c101cf04073c101cf04073c101cf040";

	    Assert.AreEqual(result, expected);
    }

    [TestMethod]
    public void IntervalPayload_MaximumAmountOfIntervalsReached()
    {
	    // 7 intervals is maximum amount
	    var intervals = GetIntervals(12);

	    Assert.ThrowsException<Exception>(() =>
		    converter.ConvertIntervalToHex(new ScheduleToSendDto() { Intervals = intervals }));
    }

    [TestMethod]
    public void IntervalPayload_HoursAndMinutesAreMinBoundaries()
    {
	    var intervals = new List<IntervalToSendDto>();
	    var interval = new IntervalToSendDto()
		    { StartTime = TimeSpan.FromHours(0) + TimeSpan.FromMinutes(0), EndTime = TimeSpan.FromHours(0) + TimeSpan.FromMinutes(0) };
	    intervals.Add(interval);

	    string result = converter.ConvertIntervalToHex(new ScheduleToSendDto(){Intervals = intervals});

	    Assert.AreEqual(result, "080000");
    }

    [TestMethod]
    public void IntervalPayload_HoursAndMinutesAreMaxBoundaries()
    {
	    var intervals = new List<IntervalToSendDto>();
	    var interval = new IntervalToSendDto()
		    { StartTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59), EndTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59) };
	    intervals.Add(interval);

	    string result = converter.ConvertIntervalToHex(new ScheduleToSendDto(){Intervals = intervals});

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
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = true
	    };
	    string result = converter.ConvertActionsPayloadToHex(dto, 16);
	    Assert.AreEqual("120010", result);
    }
    [TestMethod]
    public async Task ActionsPayload_DurationOverLimit()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = true
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 1024));

    }
    [TestMethod]
    public async Task ActionsPayload_DurationTooLow()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = true
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, -1));
    }
    [TestMethod]
    public void ActionsPayload_NullDto()
    {
        Assert.ThrowsException<NullReferenceException>(() => converter.ConvertActionsPayloadToHex(null, 1));
    }
    [TestMethod]
    public void ActionsPayload_ZeroDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = true
	    };
        Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 1025));
    }

    [TestMethod]
    public void ActionsPayload_ToggleFalseCorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = false
	    };
        Assert.AreEqual("100001", converter.ConvertActionsPayloadToHex(dto, 1));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueCorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = true
	    };
	    Assert.AreEqual("120001", converter.ConvertActionsPayloadToHex(dto, 1));
    }
    [TestMethod]
    public void ActionsPayload_ToggleFalseIncorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = false
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 1024));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueIncorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = true
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 100000));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueIncorrectNegativeDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    State = true
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, -1));
    }
    
    
    [TestMethod]
    public void testHex()
    {        
	    // Threshold threshold1 = new Threshold() { Id = 1, Type = "temperature", MinValue = 20, MaxValue = 40 };
	    // Threshold threshold2 = new Threshold() { Id = 2, Type = "humidity", MinValue = 20, MaxValue = 100 };
	    // Threshold threshold3 = new Threshold() { Id = 3, Type = "co2", MinValue = 1000, MaxValue = 1200 };
	    List<Threshold> thresholds = new List<Threshold>();
	    // thresholds.Add(threshold1);
	    // thresholds.Add(threshold2);
	    // thresholds.Add(threshold3);

	    PresetDto presetDto = new PresetDto
	    {
		    Id = 1,
		    IsCurrent = true,
		    Name = "Tomato",
		    Thresholds = thresholds
	    };
	    converter.ConvertPresetToHex(presetDto);}
    
    
    
	    [TestMethod]
	    public void ConvertPresetToHex_NullPresetDto()
	    {
		    // Arrange
		    PresetDto presetDto = null;

		    // Act & Assert
		    Assert.ThrowsException<ArgumentNullException>(() => converter.ConvertPresetToHex(presetDto));
	    }
	    
	    [TestMethod]
	    public void ConvertPresetToHex_EmptyThresholds()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto { Id = 3, Thresholds = new List<Threshold>() };

		    // Act
		    string resultHex = converter.ConvertPresetToHex(presetDto);

		    // Assert
		    Assert.AreEqual("0c0000000000000000", resultHex);
	    }
	    
	    [TestMethod]
	    public void ConvertPresetToHex_ValidPresetDto()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    //000011
			    Id = 3,
			    Thresholds = new List<Threshold>
			    {	
				    //22 bits
				    //20 = 00000010100 30 = 00000011110
				    new Threshold { Type = "temperature", MinValue = 20, MaxValue = 30 },
				    //14 bits
				    //40 = 0101000 60 = 0111100
				    new Threshold { Type = "humidity", MinValue = 40, MaxValue = 60 },
				    //24 bits
				    //100 = 000001100100 200 = 000011001000
				    new Threshold { Type = "co2", MinValue = 100, MaxValue = 200 }
			    }
		    };
		    
		    // Act
		    //00001100 00001010 00000001 11100101 00001111 00000001 10010000 00110010 00
		    // 0c        0a        01       e5       0f        01     90       32     00
		    string resultHex = converter.ConvertPresetToHex(presetDto);

		    // Assert
		    Assert.AreEqual("0c0a01e50f01903200", resultHex);
	    }
	    
	    [TestMethod]
	    public void ConvertPresetToHex_MissingThresholds()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto { Id = 3 };

		    // Act & Assert
		    var exception = Assert.ThrowsException<ArgumentNullException>(() => converter.ConvertPresetToHex(presetDto));
	    }
	    
	    [TestMethod]
	    public void ConvertPresetToHex_ThresholdWithInvalidType()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    //000011
			    Id = 3,
			    Thresholds = new List<Threshold>
			    {
				    //00 00001010 00000001 1110
				    new Threshold { Type = "temperature", MinValue = 20, MaxValue = 30 },
				    //0000 0000 0000 00
				    new Threshold { Type = "unknown", MinValue = 40, MaxValue = 60 },
				    //000001 10010000 00110010 00
				    new Threshold { Type = "co2", MinValue = 100, MaxValue = 200 }
			    }
		    };

		    // Act
		    string resultHex = converter.ConvertPresetToHex(presetDto);

		    // Assert
		    Assert.AreEqual("0c0a01e00001903200", resultHex);
	    }
	    
	    [TestMethod]
	    public void ConvertPresetToHex_ThresholdsWithNegativeValues_ReturnsDefaultHexValue()
	    {
		    // Arrange
		    PresetDto presetDto = new PresetDto
		    {
			    Id = 3,
			    Thresholds = new List<Threshold>
			    {
				    new Threshold { Type = "temperature", MinValue = -10, MaxValue = 10 },
				    //14 bits
				    //40 = 0101000 60 = 0111100
				    new Threshold { Type = "humidity", MinValue = 40, MaxValue = 60 },
				    //24 bits
				    //100 = 000001100100 200 = 000011001000
				    new Threshold { Type = "co2", MinValue = 100, MaxValue = 200 }
			    }
		    };

		    // Act
		    string resultHex = converter.ConvertPresetToHex(presetDto);

		    // Assert
		    Assert.AreEqual("0fffffffd80528780c8190", resultHex);
	    }
	    
}
