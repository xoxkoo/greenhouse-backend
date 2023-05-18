using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocketServer;
using Sprache;
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
    
    //GetCurrentAsync()
    
    //Z - Zero
    [TestMethod]
    public async Task GetCurrentAsync_ReturnsNoCurrentPreset()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = false,
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
        var response = await _controller.GetCurrentAsync();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)response.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        var result = (PresetEfcDto)okResult.Value;
        Assert.IsNull(result);
    }

    //O - One
    [TestMethod]
    public async Task GetCurrentAsync_ReturnsCurrentPreset()
    {
        // Arrange
        var preset = new Preset
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
        };
        DbContext.Presets.Add(preset);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _controller.GetCurrentAsync();

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)response.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        var result = (PresetEfcDto)okResult.Value;
        Assert.IsNotNull(result);
        Assert.AreEqual(preset.Id, result.Id);
        Assert.AreEqual(preset.Name, result.Name);
        Assert.AreEqual(preset.Thresholds.Count(), result.Thresholds.Count());
    }
    
    //CreateAsync()
    //O - One
    [TestMethod]
    public async Task CreateAsync_ReturnsCreatedPreset()
    {
        // Arrange
        var presetDto = new PresetEfcDto
        {
            Name = "New Preset",
            Thresholds = new List<ThresholdDto>
            {
                new ThresholdDto { Type = "temperature", Max = 25, Min = 20 },
                new ThresholdDto { Type = "humidity", Max = 70, Min = 60 },
                new ThresholdDto { Type = "co2", Max = 1500, Min = 1300 }
            }
        };

        // Act
        var response = await _controller.CreateAsync(presetDto);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)response.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        var result = (PresetEfcDto)okResult.Value;
        Assert.IsNotNull(result);
        Assert.AreEqual(presetDto.Name, result.Name);
        Assert.AreEqual(presetDto.Thresholds.Count(), result.Thresholds.Count());
    }

    //E - Exception
    [TestMethod]
    public async Task CreateAsync_ReturnsBadRequestOnNullDto()
    {
        // Arrange
        PresetEfcDto presetDto = null;

        // Act
        var response = await _controller.CreateAsync(presetDto);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(ObjectResult));
        var badRequestResult = (ObjectResult)response.Result;
        Assert.AreEqual(500, badRequestResult.StatusCode);
    }
    
    [TestMethod]
    public async Task CreateAsync_ReturnsCreatedPresetWithZeroThresholds()
    {
        // Arrange
        var presetDto = new PresetEfcDto
        {
            Name = "New Preset",
            Thresholds = new List<ThresholdDto>()
        };

        // Act
        var response = await _controller.CreateAsync(presetDto);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(ObjectResult));
        var okResult = (ObjectResult)response.Result;
        Assert.AreEqual(500, okResult.StatusCode);
    }
    
    
    //ApplyAsync() tests
    [TestMethod]
    public async Task ApplyAsync_ReturnsOkResult()
    {
        // Arrange
        int presetId = 1; // ID of the preset to apply
        var preset = new Preset
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
        };
        DbContext.Presets.Add(preset);
        await DbContext.SaveChangesAsync();

        // Act
        var response = await _controller.ApplyAsync(presetId);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response, typeof(OkResult));
    }
    
    
    //E - Exception
    [TestMethod]
    public async Task ApplyAsync_NoIdInDatabase()
    {
        // Arrange
        int presetId = 1; // ID of the preset to apply
        var response = await _controller.ApplyAsync(presetId);
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response, typeof(ObjectResult));
        var statusCode = (ObjectResult)response;
        Assert.AreEqual(500, statusCode.StatusCode);
    }
    
    
    //GetByIdAsync()
    [TestMethod]
    public async Task GetByIdAsync_ReturnsPresetWithMatchingId()
    {
        // Arrange
        int presetId = 1;
        var preset = new Preset
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
        };
        DbContext.Presets.Add(preset);
        await DbContext.SaveChangesAsync();
        
        // Act
        var response = await _controller.GetByIdAsync(presetId);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)response.Result;
        Assert.AreEqual(200, okResult.StatusCode);
        var result = (PresetEfcDto)okResult.Value;
        Assert.IsNotNull(result);
        Assert.AreEqual(presetId, result.Id);
    }
    
    [TestMethod]
    public async Task GetByIdAsync_NoIdInDatabase()
    {
        // Arrange
        int presetId = 999;

        // Act
        var response = await _controller.GetByIdAsync(presetId);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(ObjectResult));
        var notFoundResult = (ObjectResult)response.Result;
        Assert.AreEqual(500, notFoundResult.StatusCode);
    }
    
}