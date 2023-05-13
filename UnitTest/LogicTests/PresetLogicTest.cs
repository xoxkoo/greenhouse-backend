using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Testing.WebApiTests;

[TestClass]
public class PresetLogicTest
{
    private Mock<IPresetDao> _mockPresetDao;
    private IPresetLogic _presetLogic;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockPresetDao = new Mock<IPresetDao>();
        _presetLogic = new PresetLogic(_mockPresetDao.Object);
    }

    //Z - Zero
    [TestMethod]
    public async Task GetAsync_ZeroPresets_ReturnsEmptyPresetDtoList()
    {
        // Arrange
        var presetList = new List<PresetDto>();
        var searchPresetParametersDto = new SearchPresetParametersDto();

        _mockPresetDao
            .Setup(x => x.GetAsync(searchPresetParametersDto))
            .ReturnsAsync(presetList);

        // Act
        var result = await _presetLogic.GetAsync(searchPresetParametersDto);

        // Assert
        Assert.AreEqual(0, result.Count());
    }

    //O - One
    [TestMethod]
    public async Task GetAsync_OnePreset()
    {
        // Arrange
        var presetDto = new PresetDto { Id = 1, Name = "Tomato", IsCurrent = true };
        var presetList = new List<PresetDto> { presetDto };
        var searchPresetParametersDto = new SearchPresetParametersDto();

        _mockPresetDao
            .Setup(x => x.GetAsync(searchPresetParametersDto))
            .ReturnsAsync(presetList);

        // Act
        var result = await _presetLogic.GetAsync(searchPresetParametersDto);

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(presetDto.Id, result.First().Id);
        Assert.AreEqual(presetDto.Name, result.First().Name);
        Assert.AreEqual(presetDto.IsCurrent, result.First().IsCurrent);
    }

    //M - Many
    [TestMethod]
    public async Task GetAsync_ReturnsAllPresets()
    {
        // Arrange
        var parametersDto = new SearchPresetParametersDto();
        var presets = new List<PresetDto>
        {
            new PresetDto { Id = 1, Name = "Tomato", IsCurrent = true },
            new PresetDto { Id = 2, Name = "Sunny Day", IsCurrent = false }
        };
        _mockPresetDao.Setup(dao => dao.GetAsync(parametersDto)).ReturnsAsync(presets);

        // Act
        var result = await _presetLogic.GetAsync(parametersDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(p => p.Id == 1));
        Assert.IsTrue(result.Any(p => p.Id == 2));
    }

    [TestMethod]
    public async Task GetAsync_ReturnsPresetsByIsCurrent()
    {
        // Arrange
        var parametersDto = new SearchPresetParametersDto(null, true);
        var presets = new List<PresetDto>
        {
            new PresetDto { Id = 1, Name = "Tomato", IsCurrent = true },
            new PresetDto { Id = 2, Name = "Sunny Day", IsCurrent = false },
            new PresetDto { Id = 3, Name = "Rainy Day", IsCurrent = false }
        };
        _mockPresetDao.Setup(dao => dao.GetAsync(parametersDto)).ReturnsAsync(presets.Where(p => p.IsCurrent));

        // Act
        var result = await _presetLogic.GetAsync(parametersDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Any(p => p.Id == 1));
    }

    [TestMethod]
    public async Task GetAsync_ReturnsPresetById()
    {
        // Arrange
        var parametersDto = new SearchPresetParametersDto(1, null);
        var presets = new List<PresetDto>
        {
            new PresetDto { Id = 1, Name = "Tomato", IsCurrent = true },
            new PresetDto { Id = 2, Name = "Sunny Day", IsCurrent = false }
        };
        _mockPresetDao.Setup(dao => dao.GetAsync(parametersDto))
            .ReturnsAsync(presets.Where(p => p.Id == parametersDto.Id));

        // Act
        var result = await _presetLogic.GetAsync(parametersDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Any(p => p.Id == 1));
    }
    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenHumidityThresholdValueIsOutOfRange()
    {
        // Arrange
        PresetCreationDto dto = new PresetCreationDto
        {
            Thresholds = new List<ThresholdDto>
            {
                new ThresholdDto { Type = "Humidity", MinValue = -10, MaxValue = 120 }
            }
        };
        // Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _presetLogic.CreateAsync(dto),
            "Humidity value is ranging from 0 to 100");
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentNullException_WhenDtoIsNull()
    {
        // Arrange
        PresetCreationDto dto = null;

        // Act and Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _presetLogic.CreateAsync(dto));
        Assert.AreEqual("Provided data cannot be null (Parameter 'dto')", exception.Message);
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenThresholdsIsNull()
    {
        // Arrange
        PresetCreationDto dto = new PresetCreationDto
        {
            Thresholds = null
        };

        // Act and Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _presetLogic.CreateAsync(dto));
        Assert.AreEqual("Exactly three thresholds must be provided", exception.Message);
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenThresholdsCountIsNotThree()
    {
        // Arrange
        PresetCreationDto dto = new PresetCreationDto
        {
            Thresholds = new List<ThresholdDto> { new ThresholdDto(), new ThresholdDto() }
        };
        Preset preset = new Preset
        {
            Thresholds = new List<Threshold>
            {
                new Threshold(),
                new Threshold()
            }
        };
        PresetEfcDto efc = new PresetEfcDto
        {
            Id = 1,
            Name = "cos",
            Thresholds = dto.Thresholds
        };
        _mockPresetDao
            .Setup(x => x.CreateAsync(preset))
            .ReturnsAsync(efc);
        // Act and Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _presetLogic.CreateAsync(dto));
        string exp = "Exactly three thresholds must be provided";
        Assert.AreEqual(exp,exception.Message);
        //Assert.AreEqual("dto.Thresholds", exception.ParamName);
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenThresholdTypeIsInvalid()
    {
        // Arrange
        PresetCreationDto dto = new PresetCreationDto
        {
            Thresholds = new List<ThresholdDto>
            {
                new ThresholdDto { Type = "InvalidType", MinValue = 0, MaxValue = 10 },
                new ThresholdDto { Type = "InvalidType", MinValue = 0, MaxValue = 10 },
                new ThresholdDto { Type = "InvalidType", MinValue = 0, MaxValue = 10 }
            }
        };

        // Act and Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _presetLogic.CreateAsync(dto));
        Assert.AreEqual("There must be exactly three thresholds named: CO2, Humidity, and Temperature.", exception.Message);
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenMinValueIsBiggerThanMaxValue()
    {
        // Arrange
        PresetCreationDto dto = new PresetCreationDto
        {
            Thresholds = new List<ThresholdDto>
            {
                new ThresholdDto { Type = "CO2", MinValue = 20, MaxValue = 10 },
                new ThresholdDto { Type = "Temperature", MinValue = -10, MaxValue = 20 },
                new ThresholdDto { Type = "Humidity", MinValue = 10, MaxValue = 90 }
            }
        };

        // Act and Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _presetLogic.CreateAsync(dto));
        Assert.AreEqual("Min value cannot be bigger than max value.", exception.Message);
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenCO2ThresholdValueIsOutOfRange()
    {
        // Arrange
        PresetCreationDto dto = new PresetCreationDto
        {
            Thresholds = new List<ThresholdDto>
            {
                new ThresholdDto { Type = "CO2", MinValue = -10, MaxValue = 5000 },
                new ThresholdDto { Type = "Temperature", MinValue = -10, MaxValue = 20 },
                new ThresholdDto { Type = "Humidity", MinValue = 10, MaxValue = 90 }
            }
        };

        // Act and Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentException>(() => _presetLogic.CreateAsync(dto));
        Assert.AreEqual("CO2 value must range from 0 to 4095.", exception.Message);
    }
    [TestMethod]
    public async Task CreateAsync_ReturnsPresetEfcDto_WhenDataIsValid()
    {
        // Arrange
        PresetCreationDto dto = new PresetCreationDto
        {
            Name = "Test Preset",
            Thresholds = new List<ThresholdDto>
            {
                new ThresholdDto { Type = "CO2", MinValue = 0, MaxValue = 100 },
                new ThresholdDto { Type = "Temperature", MinValue = -10, MaxValue = 30 },
                new ThresholdDto { Type = "Humidity", MinValue = 20, MaxValue = 80 }
            }
        };

        PresetEfcDto expectedResult = new PresetEfcDto { Id = 1, Name = "Test Preset" };

        _mockPresetDao
            .Setup(x => x.CreateAsync(It.IsAny<Preset>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _presetLogic.CreateAsync(dto);

        // Assert
        Assert.AreEqual(expectedResult, result);
        _mockPresetDao.Verify(x => x.CreateAsync(It.IsAny<Preset>()), Times.Once);
    }
}