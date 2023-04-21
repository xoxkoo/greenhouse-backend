using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Mvc;
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
}