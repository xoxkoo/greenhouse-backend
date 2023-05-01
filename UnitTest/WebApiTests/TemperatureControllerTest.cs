using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;
using Exception = System.Exception;


namespace Testing.WebApiTests;
[TestClass]
public class TemperatureControllerTest
{

    [TestMethod]
    public async Task GetAsync_StartDateAfterEndDate_ReturnsBadRequest()
    {
        var expectedErrorMessage = "Start date cannot be before the end date";
        // Arrange
        var logicMock = new Mock<ITemperatureLogic>();
        logicMock
            .Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ThrowsAsync(new Exception("Start date cannot be before the end date"));

        var controller = new TemperatureController(logicMock.Object);
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
        TemperatureDto dto = new TemperatureDto(){Date = time,TemperatureId = 1,value = 50};
        IEnumerable<TemperatureDto> list = new[] { dto };
        // Arrange
        var logicMock = new Mock<ITemperatureLogic>();
        logicMock
            .Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>())).ReturnsAsync(list);
        
        var controller = new TemperatureController(logicMock.Object);
        // Act
        await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(+1));
        // Check
        Assert.AreEqual(50,dto.value);

    }
    [TestMethod]
    public async Task GetAsync_checkDate()
    {
        DateTime time = new DateTime(2001,1,1);
        TemperatureDto dto = new TemperatureDto(){Date = time,TemperatureId = 1,value = 50};
        IEnumerable<TemperatureDto> list = new[] { dto };
        // Arrange
        var logicMock = new Mock<ITemperatureLogic>();
        logicMock
            .Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>())).ReturnsAsync(list);
        
        var controller = new TemperatureController(logicMock.Object);
        // Act
        await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(+1));
        // Check
        Assert.AreEqual(new DateTime(2001,1,1),dto.Date);
    }
}