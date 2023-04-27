using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Testing.WebApiTests;

[TestClass]
public class ScheduleControllerTests
{
    private Mock<IScheduleLogic> logic;
    private ScheduleController _controller;
    
    
    [TestInitialize]
    public void TestInitialize()
    {
        logic = new Mock<IScheduleLogic>();
        _controller = new ScheduleController(logic.Object);
    }

    [TestMethod]
    public async Task CreateAsync_Returns_Created()
    {
        // Arrange
        var dto = new ScheduleCreationDto
        {
            Intervals = new List<IntervalDto>
            {
                new IntervalDto
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(9, 0, 0)
                }
            }
        };

        var fromLogic = new ScheduleDto()
        {
            Id = 1,
            Intervals = new List<IntervalDto>
            {
                new IntervalDto
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(9, 0, 0)
                }
            }
        };
        logic.Setup(x => x.CreateAsync(dto))
            .ReturnsAsync(fromLogic);
        // Act
        ActionResult<ScheduleDto> response = await _controller.CreateAsync(dto);
        Debug.WriteLine(response);
        
        
        // Assert
        Assert.IsInstanceOfType(response.Result, typeof(CreatedResult));
        CreatedResult createdResult = (CreatedResult)response.Result;
        Assert.AreEqual("/schedule/" + fromLogic.Id, createdResult.Location);
        Assert.AreEqual(fromLogic, createdResult.Value);
    }
    
    [TestMethod]
    public async Task CreateAsync_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        ScheduleCreationDto dto = new ScheduleCreationDto(); // Set up a valid DTO.
        logic.Setup(x => x.CreateAsync(dto)).ThrowsAsync(new Exception("An error occurred."));

        // Act
        ActionResult<ScheduleDto> result = await _controller.CreateAsync(dto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        ObjectResult statusCodeResult = (ObjectResult)result.Result;
        Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }
}