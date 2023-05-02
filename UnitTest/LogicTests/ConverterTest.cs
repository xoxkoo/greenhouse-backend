using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.WebApiTests;

[TestClass]
public class ConverterTest : DbTestBase
{

	private readonly Mock<ITemperatureLogic> tempLogic;
	private readonly Mock<IHumidityLogic> humidityLogic;
	private readonly Mock<ICO2Logic> co2logic;
	private readonly Mock<IWateringSystemLogic> waterLogic;
	private readonly IConverter converter;


    public ConverterTest()
    {
	    tempLogic = new Mock<ITemperatureLogic>();
	    co2logic = new Mock<ICO2Logic>();
	    humidityLogic = new Mock<IHumidityLogic>();
	    waterLogic = new Mock<IWateringSystemLogic>();
        converter = new Converter(tempLogic.Object, co2logic.Object, humidityLogic.Object, waterLogic.Object);
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

	    Assert.AreEqual("25,8, 31, 1279", result);
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
	    string expected = "09cf04073c101cf04073c101cf04073c101cf0400";


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

	    Assert.AreEqual(result, "0800000");
    }

    [TestMethod]
    public void IntervalPayload_HoursAndMinutesAreMaxBoundaries()
    {
	    var intervals = new List<IntervalToSendDto>();
	    var interval = new IntervalToSendDto()
		    { StartTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59), EndTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59) };
	    intervals.Add(interval);

	    string result = converter.ConvertIntervalToHex(new ScheduleToSendDto(){Intervals = intervals});

	    Assert.AreEqual(result, "0afddfb");
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
		    Toggle = true
	    };
	    string result = converter.ConvertActionsPayloadToHex(dto, 16);
	    Assert.AreEqual("120010", result);
    }
    [TestMethod]
    public async Task ActionsPayload_DurationOverLimit()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    Toggle = true
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 1024));

    }
    [TestMethod]
    public async Task ActionsPayload_DurationTooLow()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    Toggle = true
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
		    Toggle = true
	    };
        Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 1025));
    }

    [TestMethod]
    public void ActionsPayload_ToggleFalseCorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    Toggle = false
	    };
        Assert.AreEqual("100001", converter.ConvertActionsPayloadToHex(dto, 1));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueCorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    Toggle = true
	    };
	    Assert.AreEqual("120001", converter.ConvertActionsPayloadToHex(dto, 1));
    }
    [TestMethod]
    public void ActionsPayload_ToggleFalseIncorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    Toggle = false
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 1024));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueIncorrectDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    Toggle = true
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, 100000));
    }
    [TestMethod]
    public void ActionsPayload_ToggleTrueIncorrectNegativeDuration()
    {
	    ValveStateDto dto = new ValveStateDto()
	    {
		    Toggle = true
	    };
	    Assert.ThrowsException<Exception>(() => converter.ConvertActionsPayloadToHex(dto, -1));
    }
}
