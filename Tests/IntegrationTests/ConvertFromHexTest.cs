using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocketServer;
using Tests.Utils;

namespace Tests.IntegrationTests;

[TestClass]
public class ConvertFromHexTest : DbTestBase
{

    private ITemperatureLogic _temperatureLogic;
    private ICO2Logic _co2Logic;
    private IHumidityLogic _humidityLogic;
    private IWateringSystemLogic _waterLogic;
    private IEmailLogic _emailLogic;
    private IConverter _converter;
    private Mock<IWebSocketServer> socket;

    [TestInitialize]
    public void TestInitialize()
    {
	    var services = new ServiceCollection();
	    // Register DbContext and other dependencies
	    services.AddScoped<Context>(provider => DbContext);

	    // Register services from the Startup class
	    var startup = new Startup();
	    startup.ConfigureServices(services);

	    // Resolve Converter using dependency injection
	    _converter = services.BuildServiceProvider().GetService<IConverter>();

    }



    [TestMethod]
    public async Task ConvertFromHex_WhenCalledWithValidPayload_Test()
    {
	    //Assert
	    IList<Threshold> thresholds = new List<Threshold>();
	    Threshold temp = new Threshold
	    {
		    Id = 1,
		    MinValue = 20,
		    MaxValue = 21,
		    Type = "temperature"
	    };
	    Threshold humidity = new Threshold
	    {
		    Id = 2,
		    MinValue = 30,
		    MaxValue = 35,
		    Type = "humidity"
	    };Threshold co2 = new Threshold
	    {
		    Id = 3,
		    MinValue = 1200,
		    MaxValue = 1300,
		    Type = "co2"
	    };
	    thresholds.Add(temp);
	    thresholds.Add(humidity);
	    thresholds.Add(co2);
	    Preset preset = new Preset
	    {
		    Id = 1,
		    IsCurrent = true,
		    Name = "Tomato",
		    Thresholds = thresholds
	    };
	    NotificationEmail notificationEmail = new NotificationEmail
	    {
		    Email = "greenhousesep4@gmail.com"
	    };
	    await DbContext.NotificationEmails.AddAsync(notificationEmail);
	    await DbContext.Presets.AddAsync(preset);
	    await DbContext.SaveChangesAsync();
	    var valve = DbContext.ValveState;

	    string result = await _converter.ConvertFromHex("07817b0707f0");

	    // Check if records were created in the database
	    var temperatureRecord = await DbContext.Temperatures.FirstOrDefaultAsync();
	    var co2Record = await DbContext.CO2s.FirstOrDefaultAsync();
	    var humidityRecord = await DbContext.Humidities.FirstOrDefaultAsync();
	    var state = await valve.FirstOrDefaultAsync();

	    Assert.IsNotNull(temperatureRecord);
	    Assert.IsNotNull(co2Record);
	    Assert.IsNotNull(humidityRecord);
	    Assert.AreEqual("25.8, 5, 2032", result);
	    Assert.AreEqual(false, state.Toggle);
    }

    [TestMethod]
    public async Task ConvertFromHex_ValveStateOpen()
    {

	    string result = await _converter.ConvertFromHex("04047b0707f0");
	    var valve = await DbContext.ValveState.FirstOrDefaultAsync();
	    Assert.AreEqual(true, valve.Toggle);
    }

    [TestMethod]
    public async Task ConvertFromHex_ValveStateClose()
    {

	    string result = await _converter.ConvertFromHex("04017b0707f0");
	    var valve = await DbContext.ValveState.FirstOrDefaultAsync();
	    Assert.AreEqual(false, valve.Toggle);
    }

    [TestMethod]
    public async Task ConvertFromHex_ValveStateOpenThenClose()
    {

	    // open
	    await _converter.ConvertFromHex("04047b0707f0");

	    var valve = await DbContext.ValveState.OrderByDescending(v=>v.Id).Select(v => new ValveStateDto() { State = v.Toggle })
		    .FirstOrDefaultAsync();
	    Assert.AreEqual(true, valve.State);

	    //close
	    await _converter.ConvertFromHex("04017b0707f0");
	    var valve2 = await DbContext.ValveState.OrderByDescending(v=>v.Id).Select(v => new ValveStateDto() { State = v.Toggle })
		    .FirstOrDefaultAsync();
	    Assert.AreEqual(true, valve.State);
	    Console.WriteLine(valve2.State);

	    Assert.AreEqual(false, valve2.State);
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
}
