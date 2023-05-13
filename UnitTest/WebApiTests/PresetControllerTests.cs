using System.Runtime.CompilerServices;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Testing.WebApiTests;

[TestClass]
public class PresetControllerTests
{
    private Mock<IPresetLogic> _mockPresetLogic;
    private PresetController _presetController;
    
    
    [TestInitialize]
    public void TestInitialize()
    {
        _mockPresetLogic = new Mock<IPresetLogic>();
        _presetController = new PresetController(_mockPresetLogic.Object);
    }

    [TestMethod]
    public async Task GetAsync_ReturnsOkResult()
    {
        // Arrange
        var presetDtos = new List<PresetDto>
        {
            new PresetDto
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 100, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 50, MinValue = 0 },
                }
            },
            new PresetDto
            {
                Id = 2,
                Name = "Sunny Day",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 3, Type = "temperature", MaxValue = 200, MinValue = 0 },
                    new Threshold { Id = 4, Type = "humidity", MaxValue = 100, MinValue = 0 },
                }
            },
        };
        _mockPresetLogic.Setup(p => p.GetAsync(It.IsAny<SearchPresetParametersDto>())).ReturnsAsync(presetDtos);
    
        // Act
        var result = await _presetController.GetAsync();
    
        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okObjectResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okObjectResult);
        var presets = okObjectResult.Value as IEnumerable<PresetDto>;
        Assert.IsNotNull(presets);
        Assert.AreEqual(presetDtos.Count, presets.Count()); 
    }
    
    [TestMethod]
    public async Task GetAsync_ReturnsInternalServerError_OnException()
    {
        // Arrange
        _mockPresetLogic.Setup(p => p.GetAsync(It.IsAny<SearchPresetParametersDto>())).ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _presetController.GetAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        ObjectResult statusCodeResult = (ObjectResult)result.Result;  
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    
    [TestMethod]
    public async Task GetCurrentAsync_ReturnsOkResult()
    {
        // Arrange
        var presetDtos = new List<PresetDto>
        {
            new PresetDto
            {
                Id = 2,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 3, Type = "temperature", MaxValue = 200, MinValue = 0 },
                    new Threshold { Id = 4, Type = "humidity", MaxValue = 100, MinValue = 0 },
                }
            },
        };
        _mockPresetLogic.Setup(p => p.GetAsync(It.IsAny<SearchPresetParametersDto>())).ReturnsAsync(presetDtos);

        // Act
        var result = await _presetController.GetCurrentAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.AreEqual(presetDtos.FirstOrDefault(), okResult.Value);
        Assert.IsNotNull(okResult);
        var preset = okResult.Value as PresetDto;
        Assert.IsNotNull(preset);
    }
    
    [TestMethod]
    public async Task GetCurrentAsync_ReturnsInternalServerError_OnException()
    {
        // Arrange
        _mockPresetLogic.Setup(p => p.GetAsync(It.IsAny<SearchPresetParametersDto>())).ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _presetController.GetCurrentAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        ObjectResult statusCodeResult = (ObjectResult)result.Result;  
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    [TestMethod]
public async Task CreateAsync_ReturnsOkResult()
{
    // Arrange
    var presetCreationDto = new PresetCreationDto
    {
        Name = "Test Preset",
        Thresholds = new List<ThresholdDto>
        {
            new ThresholdDto {  Type = "temperature", MaxValue = 100, MinValue = 0 },
            new ThresholdDto {Type = "humidity", MaxValue = 50, MinValue = 0 }
        }
    };
    var presetEfcDto = new PresetEfcDto
    {
        Id = 1,
        Name = "Test Preset",
        Thresholds = new List<ThresholdDto>
        {
            new ThresholdDto { Type = "temperature", MaxValue = 100, MinValue = 0 },
            new ThresholdDto { Type = "humidity", MaxValue = 50, MinValue = 0 }
        }
    };
    _mockPresetLogic.Setup(p => p.CreateAsync(presetCreationDto)).ReturnsAsync(presetEfcDto);

    // Act
    var result = await _presetController.CreateAsync(presetCreationDto);

    // Assert
    Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    var okObjectResult = result.Result as OkObjectResult;
    Assert.IsNotNull(okObjectResult);
    var createdPreset = okObjectResult.Value as PresetEfcDto;
    Assert.IsNotNull(createdPreset);
    Assert.AreEqual(presetEfcDto.Id, createdPreset.Id);
    Assert.AreEqual(presetEfcDto.Name, createdPreset.Name);
    // Assert other properties and thresholds as needed
}

[TestMethod]
public async Task CreateAsync_ReturnsInternalServerError_OnException()
{
    // Arrange
    var presetCreationDto = new PresetCreationDto
    {
        Name = "Test Preset",
        Thresholds = new List<ThresholdDto>
        {
            new ThresholdDto { Type = "temperature", MaxValue = 100, MinValue = 0 },
            new ThresholdDto {Type = "humidity", MaxValue = 50, MinValue = 0 }
        }
    };
    _mockPresetLogic.Setup(p => p.CreateAsync(presetCreationDto)).ThrowsAsync(new Exception("Something went wrong"));

    // Act
    var result = await _presetController.CreateAsync(presetCreationDto);

    // Assert
    Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
    var statusCodeResult = result.Result as ObjectResult;
    Assert.IsNotNull(statusCodeResult);
    Assert.AreEqual(500, statusCodeResult.StatusCode);
}
}