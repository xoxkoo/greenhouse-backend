using Application.LogicInterfaces;
using Domain.DTOs;
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

    // [TestMethod]
    // public async Task GetAsync_ReturnsOkResult()
    // {
    //     // Arrange
    //     SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(null, null);
    //     var presetDtos = new List<PresetDto>
    //     {
    //         new PresetDto
    //         {
    //             Id = 1,
    //             Name = "Tomato",
    //             IsCurrent = false,
    //             Thresholds = new List<Threshold>
    //             {
    //                 new Threshold { Id = 1, Type = "temperature", MaxValue = 100, MinValue = 0 },
    //                 new Threshold { Id = 2, Type = "humidity", MaxValue = 50, MinValue = 0 },
    //             }
    //         },
    //         new PresetDto
    //         {
    //             Id = 2,
    //             Name = "Sunny Day",
    //             IsCurrent = true,
    //             Thresholds = new List<Threshold>
    //             {
    //                 new Threshold { Id = 3, Type = "temperature", MaxValue = 200, MinValue = 0 },
    //                 new Threshold { Id = 4, Type = "humidity", MaxValue = 100, MinValue = 0 },
    //             }
    //         },
    //     };
    //     _mockPresetLogic.Setup(p => p.GetAsync(parametersDto)).ReturnsAsync(presetDtos);
    //
    //     // Act
    //     var result = await _presetController.GetAsync();
    //
    //     // Assert
    //     Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    //     var okObjectResult = result.Result as OkObjectResult;
    //     Assert.IsNotNull(okObjectResult);
    //     var presets = okObjectResult.Value as IEnumerable<PresetDto>;
    //     Assert.IsNotNull(presets);
    //     Assert.AreEqual(presetDtos.Count, presets.Count()); 
    // }
    
    
}