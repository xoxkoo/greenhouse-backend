using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Tests.UnitTests.WebApiTests;
[TestClass]
public class WateringSystemControllerTest
{
    [TestMethod]
    public async Task CheckDurationValue()
    {
        var expectedErrorMessage = "Duration cannot be 0 or less";
        ValveStateCreationDto dto = new ValveStateCreationDto(){duration = 0,State = true};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new Exception("Duration cannot be 0 or less"));

        var controller = new WateringSystemController(logicMock.Object);

        try
        {
            await controller.PostAsync(dto);
        }
        catch (Exception e)
        {
            // Check
            Assert.AreEqual(expectedErrorMessage,e.Message);
        }
    }
    [TestMethod]
    public async Task CheckDurationMinusValue()
    {
        var expectedErrorMessage = "Duration cannot be 0 or less";
        ValveStateCreationDto dto = new ValveStateCreationDto(){duration = -1,State = true};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new Exception("Duration cannot be 0 or less"));

        var controller = new WateringSystemController(logicMock.Object);

        try
        { 
            await controller.PostAsync(dto);
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
        var expectedErrorMessage = "State has to be set";
        ValveStateCreationDto dto = new ValveStateCreationDto(){duration = 5};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new Exception("State has to be set"));

        var controller = new WateringSystemController(logicMock.Object);

        try
        {
            await controller.PostAsync(dto);
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
        ValveStateCreationDto dto = new ValveStateCreationDto(){State = false};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.CreateAsync(dto))
            .ThrowsAsync(new Exception("Duration has to be set"));

        var controller = new WateringSystemController(logicMock.Object);

        try
        {
            await controller.PostAsync(dto);
        }
        catch (Exception e)
        {
            // Check
            Assert.AreEqual(expectedErrorMessage,e.Message);
        }
    }
    [TestMethod]
    public async Task GetAsync_checkValueTrue()
    {
        ValveStateDto dto = new ValveStateDto(){State = true};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.GetAsync()).ReturnsAsync(dto);
        var controller = new WateringSystemController(logicMock.Object);
        await controller.GetAsync();
        Assert.AreEqual(true,dto.State);
 
    }
    [TestMethod]
    public async Task GetAsync_checkValueFalse()
    {
        ValveStateDto dto = new ValveStateDto(){State = false};
        // Arrange
        var logicMock = new Mock<IWateringSystemLogic>();
        logicMock
            .Setup(x => x.GetAsync()).ReturnsAsync(dto);
        var controller = new WateringSystemController(logicMock.Object);
        await controller.GetAsync();
        Assert.AreEqual(false,dto.State);
 
    }
}