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

	private readonly Mock<ITemperatureLogic> _tempLogic;
	private readonly Mock<IHumidityLogic> _humidityLogic;
	private readonly Mock<ICO2Logic> _co2Logic;
	private readonly IConverter _converter;


    public ConverterTest()
    {
	    _tempLogic = new Mock<ITemperatureLogic>();
	    _co2Logic = new Mock<ICO2Logic>();
	    _humidityLogic = new Mock<IHumidityLogic>();
        _converter = new Converter(_tempLogic.Object, _co2Logic.Object, _humidityLogic.Object);
    }

    [TestMethod]
    public async Task THCPayload_SavedToDB()
    {
	    await _converter.ConvertFromHex("07817b1f4ff0");

        // Assert
        _tempLogic.Verify(x => x.CreateAsync(It.Is<TemperatureCreateDto>(dto =>
	        // tolerance because of rounding problems
	        Math.Abs(dto.Value - 25.8) < 0.1
        )), Times.Once);

        _co2Logic.Verify(x => x.CreateAsync(It.Is<CO2CreateDto>(dto =>
	        dto.Value == 1279
        )), Times.Once);

        _humidityLogic.Verify(x => x.CreateAsync(It.Is<HumidityCreationDto>(dto =>
	        dto.Value == 31
        )), Times.Once);
    }

    [TestMethod]
    public async Task THCPayload_ResponseStringIsCorrect()
    {
	    string result = await _converter.ConvertFromHex("07817b1f4ff0");

	    Assert.AreEqual("25.8, 31, 1279", result);
    }

    [TestMethod]
    public void THCPayload_ThrowErrorWhenNotHexValue()
    {
	    Assert.ThrowsExceptionAsync<Exception>(() => _converter.ConvertFromHex("t7800c9401e0"));
    }

    [TestMethod]
    public async Task THCPayload_IncorrectValue()
    {
	    await Assert.ThrowsExceptionAsync<Exception>(() => _converter.ConvertFromHex(""));
	    await Assert.ThrowsExceptionAsync<Exception>(() => _converter.ConvertFromHex("    "));
    }

    [TestMethod]
    public void IntervalPayload_DataAreSent()
    {
	    var intervals = GetIntervals(7);


	    string result = _converter.ConvertIntervalToHex(new ScheduleDto(){Schedule = intervals});
	    string expected = "09cf04073c101cf04073c101cf04073c101cf0400";


	    Assert.AreEqual(result, expected);
    }

    [TestMethod]
    public void IntervalPayload_MaximumAmountOfIntervalsReached()
    {
	    // 7 intervals is maximum amount
	    var intervals = GetIntervals(12);

	    Assert.ThrowsException<Exception>(() =>
		    _converter.ConvertIntervalToHex(new ScheduleDto() { Schedule = intervals }));
    }

    [TestMethod]
    public void IntervalPayload_HoursAndMinutesAreMinBoundaries()
    {
	    var intervals = new List<IntervalDto>();
	    var interval = new IntervalDto()
		    { DayOfWeek = DayOfWeek.Monday, StartTime = TimeSpan.FromHours(0) + TimeSpan.FromMinutes(0), EndTime = TimeSpan.FromHours(0) + TimeSpan.FromMinutes(0) };
	    intervals.Add(interval);

	    string result = _converter.ConvertIntervalToHex(new ScheduleDto(){Schedule = intervals});

	    Assert.AreEqual(result, "0800000");
    }

    [TestMethod]
    public void IntervalPayload_HoursAndMinutesAreMaxBoundaries()
    {
	    var intervals = new List<IntervalDto>();
	    var interval = new IntervalDto()
		    { DayOfWeek = DayOfWeek.Monday, StartTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59), EndTime = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59) };
	    intervals.Add(interval);

	    string result = _converter.ConvertIntervalToHex(new ScheduleDto(){Schedule = intervals});

	    Assert.AreEqual(result, "0afddfb");
    }


    private IEnumerable<IntervalDto> GetIntervals(int n)
    {
	    var intervals = new List<IntervalDto>();
	    for (int i = 0; i < n; i++)
	    {
		    var interval = new IntervalDto()
			    { DayOfWeek = DayOfWeek.Monday, StartTime = TimeSpan.FromHours(14) + TimeSpan.FromMinutes(30), EndTime = TimeSpan.FromHours(1) + TimeSpan.FromMinutes(0) };
		    intervals.Add(interval);

	    }
	    return intervals;
    }

}
