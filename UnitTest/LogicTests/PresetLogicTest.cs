using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
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
        _mockPresetDao.Setup(dao => dao.GetAsync(parametersDto)).ReturnsAsync(presets.Where(p => p.Id == parametersDto.Id));
        
        // Act
        var result = await _presetLogic.GetAsync(parametersDto);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Any(p => p.Id == 1));
    }
}