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
        var presetDtos = new List<PresetEfcDto>
        {
            new PresetEfcDto
            {
                Id = 1,
                Name = "Tomato",
                Thresholds = new List<ThresholdDto>
                {
                    new ThresholdDto() { Type = "temperature", Max = 100, Min = 0 },
                    new ThresholdDto() {Type = "humidity", Max = 50, Min = 0 },
                }
            },
            new PresetEfcDto()
            {
                Id = 2,
                Name = "Sunny Day",
                Thresholds = new List<ThresholdDto>
                {
                    new ThresholdDto() { Type = "temperature", Max = 200, Min = 0 },
                    new ThresholdDto(){ Type = "humidity", Max = 100, Min = 0 },
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
        var presets = okObjectResult.Value as IEnumerable<PresetEfcDto>;
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
        var presetDtos = new List<PresetEfcDto>
        {
            new PresetEfcDto()
            {
                Id = 2,
                Name = "Sunny Day",
                Thresholds = new List<ThresholdDto>
                {
                    new ThresholdDto() { Type = "temperature", Max = 200, Min = 0 },
                    new ThresholdDto(){ Type = "humidity", Max = 100, Min = 0 },
                }
            },
        };
        _mockPresetLogic.Setup(p => p.GetAsync(It.IsAny<SearchPresetParametersDto>())).ReturnsAsync(presetDtos);

        // Act
        var result = await _presetController.GetCurrentAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var preset = okResult.Value as PresetEfcDto;
        Assert.IsNotNull(preset);
        Assert.AreEqual(presetDtos.FirstOrDefault().Id, preset.Id);
        Assert.AreEqual(presetDtos.FirstOrDefault().Name, preset.Name);
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
                new ThresholdDto {  Type = "temperature", Max = 100, Min = 0 },
                new ThresholdDto {Type = "humidity", Max = 50, Min = 0 }
            }
        };
        var presetEfcDto = new PresetEfcDto
        {
            Id = 1,
            Name = "Test Preset",
            Thresholds = new List<ThresholdDto>
            {
                new ThresholdDto { Type = "temperature", Max = 100, Min = 0 },
                new ThresholdDto { Type = "humidity", Max = 50, Min = 0 }
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
                new ThresholdDto { Type = "temperature", Max = 100, Min = 0 },
                new ThresholdDto {Type = "humidity", Max = 50, Min = 0 }
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
    
    //ApplyAsync()
    [TestMethod]
    public async Task ApplyAsync_ReturnsOkResult()
    {
        // Act
        var result = await _presetController.ApplyAsync(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public async Task ApplyAsync_ReturnsInternalServerError()
    {
        // Arrange
        int presetId = 1;
        _mockPresetLogic.Setup(p => p.ApplyAsync(presetId)).ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _presetController.ApplyAsync(presetId);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));
        var statusCodeResult = (ObjectResult)result;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    
    [TestMethod]
    public async Task ApplyAsync_CallsPresetLogicApplyAsync()
    {
        // Arrange
        int presetId = 1;

        // Act
        await _presetController.ApplyAsync(presetId);

        // Assert
        _mockPresetLogic.Verify(p => p.ApplyAsync(presetId), Times.Once);
    }
}