using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocketServer;
using Testing.Utils;
using WebAPI.Controllers;

namespace Testing.IntegrationTests;

[TestClass]
public class PresetIntegrationTest : DbTestBase
{
	private PresetController _controller;

    [TestInitialize]
    public void SetUp()
    {
	    var services = new ServiceCollection();
	    // Register DbContext and other dependencies
	    services.AddScoped<Context>(provider => DbContext);

	    // Register services from the Startup class
	    var startup = new Startup();
	    startup.ConfigureServices(services);

	    // Resolve PresetController using dependency injection
	    _controller = services.BuildServiceProvider().GetService<PresetController>();
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
        var presetDto = new PresetCreationDto()
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
        Assert.IsInstanceOfType(response.Result, typeof(CreatedResult));
        var okResult = (CreatedResult)response.Result;
        Assert.AreEqual(201, okResult.StatusCode);
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
        PresetCreationDto presetDto = null;

        // Act
        var response = await _controller.CreateAsync(presetDto);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(ObjectResult));
        var badRequestResult = (ObjectResult)response.Result;
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task CreateAsync_ReturnsCreatedPresetWithZeroThresholds()
    {
        // Arrange
        var presetDto = new PresetCreationDto()
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
        Assert.AreEqual(400, okResult.StatusCode);
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
        var response = await _controller.ApplyAsync(new PresetApplyDto(){Id = presetId});

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
        var response = await _controller.ApplyAsync(new PresetApplyDto(){Id = presetId});
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
        var result = okResult.Value as PresetEfcDto;
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
