
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.Entities;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SocketServer;
using Testing.Utils;

namespace Testing.IntegrationTests;

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
	    string result = await _converter.ConvertFromHex("07817b0707f0");

	    // Check if records were created in the database
	    var temperatureRecord = await DbContext.Temperatures.FirstOrDefaultAsync();
	    var co2Record = await DbContext.CO2s.FirstOrDefaultAsync();
	    var humidityRecord = await DbContext.Humidities.FirstOrDefaultAsync();

	    Assert.IsNotNull(temperatureRecord);
	    Assert.IsNotNull(co2Record);
	    Assert.IsNotNull(humidityRecord);
	    Assert.AreEqual("25.8, 56, 2032", result);
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
