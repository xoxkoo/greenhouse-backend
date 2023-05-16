using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocketServer;
using Testing.Utils;
using WebAPI.Controllers;

namespace Testing.IntegrationTests;

[TestClass]
public class PresetIntegrationTest : DbTestBase
{
    private IPresetDao _presetDao;
    private IWebSocketServer _socketServer;
    private IHumidityDao _humidityDao;
    private ICO2Dao _co2Dao;
    private ITemperatureDao _temperatureDao;
    private IEmailDao _emailDao;
    
    private IHumidityLogic _humidityLogic;
    private ITemperatureLogic _temperatureLogic;
    private ICO2Logic _co2Logic;
    private IEmailLogic _emailLogic;
    private IConverter _converter;
    private IPresetLogic _presetLogic;
    
    private PresetController _controller;
    
    [TestInitialize]
    public void SetUp()
    {
        _presetDao = new PresetEfcDao(DbContext);
        _humidityDao = new HumidityEfcDao(DbContext);
        _temperatureDao = new TemperatureEfcDao(DbContext);
        _co2Dao = new CO2EfcDao(DbContext);
        _emailDao = new EmailEfcDao(DbContext);
        _socketServer = new WebSocketServer();
        _humidityLogic = new HumidityLogic(_humidityDao);
        _temperatureLogic = new TemperatureLogic(_temperatureDao);
        _co2Logic = new CO2Logic(_co2Dao);
        _emailLogic = new EmailLogic(_emailDao, _presetDao);
        _converter = new Converter(_temperatureLogic, _co2Logic, _humidityLogic, _emailLogic);
        _presetLogic = new PresetLogic(_presetDao, _socketServer, _converter);
        _controller = new PresetController(_presetLogic);
    }

    //GetAsync() tests
    //Z - Zero
    [TestMethod]
    public async Task GetAsync_ReturnsPresetListEmpty()
    {
        // Act
        var response = await _controller.GetAsync();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)response.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        var result = (IEnumerable<PresetEfcDto>)okResult.Value;
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    //O - One
    [TestMethod]
    public async Task GetAsync_ReturnsPresetListWithOneElement()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            }
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _controller.GetAsync();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)response.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        var result = (IEnumerable<PresetEfcDto>)okResult.Value;
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        
    }
    
    
    //M - Many
    [TestMethod]
    public async Task GetAsync_ReturnsPresetList_Test()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            },
            new Preset
            {
                Id = 2,
                Name = "Sunny Day",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 4, Type = "temperature", MaxValue = 13, MinValue = 0 },
                    new Threshold { Id = 5, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 6, Type = "co2", MaxValue = 1250, MinValue = 1230 }
                }
            },
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _controller.GetAsync();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)response.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        var result = (IEnumerable<PresetEfcDto>)okResult.Value;
        Assert.IsNotNull(result);
        Assert.AreEqual(presets.Count, result.Count());
    }
    
    
    
}