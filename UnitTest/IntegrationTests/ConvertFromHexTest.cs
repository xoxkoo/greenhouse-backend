using System.Collections;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

    [TestInitialize]
    public void TestInitialize()
    {
        ITemperatureDao temperatureDao = new TemperatureEfcDao(DbContext);
        ICO2Dao co2Dao = new CO2EfcDao(DbContext);
        IHumidityDao humidityDao = new HumidityEfcDao(DbContext);
        IWateringSystemDao wateringSystemDao = new WateringSystemDao(DbContext);
        IEmailDao emailDao = new EmailEfcDao(DbContext);
        IPresetDao presetDao = new PresetEfcDao(DbContext);
        _temperatureLogic = new TemperatureLogic(temperatureDao);
        _co2Logic = new CO2Logic(co2Dao);
        _humidityLogic = new HumidityLogic(humidityDao);
        _waterLogic = new WateringSystemLogic(wateringSystemDao);
        _emailLogic = new EmailLogic(emailDao, presetDao);
        _converter = new Converter(_temperatureLogic, _co2Logic, _humidityLogic, _waterLogic, _emailLogic);
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
	    Email email = new Email
	    {
		    EmailAddress = "natakoziara@gmail.com"
	    };
	    await DbContext.Mails.AddAsync(email);
	    await DbContext.Presets.AddAsync(preset);
	    await DbContext.SaveChangesAsync();
	    string result = await _converter.ConvertFromHex("07817b1f4ff0");

	    // Check if records were created in the database
	    var temperatureRecord = await DbContext.Temperatures.FirstOrDefaultAsync();
	    var co2Record = await DbContext.CO2s.FirstOrDefaultAsync();
	    var humidityRecord = await DbContext.Humidities.FirstOrDefaultAsync();

	    Assert.IsNotNull(temperatureRecord);
	    Assert.IsNotNull(co2Record);
	    Assert.IsNotNull(humidityRecord);
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
}
