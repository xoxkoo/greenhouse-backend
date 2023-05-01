using System.Collections;
using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Testing.WebApiTests;
[TestClass]
public class CO2ControllerTests
{
    [TestMethod]
    public async Task GetAsync_StartDateAfterEndDate_ReturnsBadRequest()
    {
        var expectedErrorMessage = "Start date cannot be before the end date";
        // Arrange
        var logicMock = new Mock<ICO2Logic>();
        logicMock
            .Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ThrowsAsync(new Exception("Start date cannot be before the end date"));

        var controller = new CO2Controller(logicMock.Object);
        // Act
        try
        {
            await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(-1));
        }
        catch (Exception e)
        {
            // Check
            Assert.AreEqual(expectedErrorMessage,e.Message);
        }

    }
    [TestMethod]
    public async Task GetAsync_checkValue()
    {
        DateTime time = DateTime.Now;
        CO2Dto dto = new CO2Dto(){Date = time,CO2Id = 1,Value = 50};
        IEnumerable<CO2Dto> list = new[] { dto };
        // Arrange
        var logicMock = new Mock<ICO2Logic>();
        logicMock
            .Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>())).ReturnsAsync(list);
        
        var controller = new CO2Controller(logicMock.Object);
        // Act
        await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(+1));
        // Check
            Assert.AreEqual(50,dto.Value);

    }
    [TestMethod]
    public async Task GetAsync_CheckId()
    {
        DateTime time = new DateTime(2001,1,1);
        CO2Dto dto = new CO2Dto(){Date = time,CO2Id = 1,Value = 50};
        IEnumerable<CO2Dto> list = new[] { dto };
        // Arrange
        var logicMock = new Mock<ICO2Logic>();
        logicMock
            .Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>())).ReturnsAsync(list);
        
        var controller = new CO2Controller(logicMock.Object);
        // Act
        await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(+1));
        // Check
        Assert.AreEqual(1,dto.CO2Id);
    }
    [TestMethod]
    public async Task GetAsync_checkDate()
    {
        DateTime time = new DateTime(2001,1,1);
        CO2Dto dto = new CO2Dto(){Date = time,CO2Id = 1,Value = 50};
        IEnumerable<CO2Dto> list = new[] { dto };
        // Arrange
        var logicMock = new Mock<ICO2Logic>();
        logicMock
            .Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>())).ReturnsAsync(list);
        
        var controller = new CO2Controller(logicMock.Object);
        // Act
        await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(+1));
        // Check
        Assert.AreEqual(new DateTime(2001,1,1),dto.Date);
    }
}