using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Testing.WebApiTests;
[TestClass]
public class WateringSystemTest
{
    [TestMethod]
    public async Task CheckDurationValue()
    {
        var expectedErrorMessage = "Duration cannot be 0 or less";
        ValveStateCreationDto dto = new ValveStateCreationDto(){duration = 0,Toggle = true};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new Exception("Duration cannot be 0 or less"));

        var controller = new WateringSystemController(logicMock.Object);

        try
        {
            await controller.PostAsync(true,2);
        }
        catch (Exception e)
        {
            // Check
            Assert.AreEqual(expectedErrorMessage,e.Message);
        }
    }
    /** the valve state creation is being set by only duration so that the toggle is null*/
    [TestMethod]
    public async Task CheckToggle()
    {
        var expectedErrorMessage = "Toggle has to be set";
        ValveStateCreationDto dto = new ValveStateCreationDto(){duration = 5};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new Exception("Toggle has to be set"));

        var controller = new WateringSystemController(logicMock.Object);

        try
        {
            await controller.PostAsync(true,2);
        }
        catch (Exception e)
        {
            // Check
            Assert.AreEqual(expectedErrorMessage,e.Message);
        }
    }
    /** the valve state creation is being set by only toggle so that the duration is null*/
    [TestMethod]
    public async Task CheckDuration()
    {
        var expectedErrorMessage = "Duration has to be set";
        ValveStateCreationDto dto = new ValveStateCreationDto(){Toggle = false};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new Exception("Duration has to be set"));

        var controller = new WateringSystemController(logicMock.Object);

        try
        {
            await controller.PostAsync(true,2);
        }
        catch (Exception e)
        {
            // Check
            Assert.AreEqual(expectedErrorMessage,e.Message);
        }
    }
}