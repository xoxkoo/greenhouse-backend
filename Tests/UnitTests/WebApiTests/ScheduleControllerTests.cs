using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Tests.UnitTests.WebApiTests;

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
	    var intervals = new List<IntervalDto>
	    {
		    new IntervalDto
		    {
			    DayOfWeek = DayOfWeek.Monday,
			    StartTime = new TimeSpan(8, 0, 0),
			    EndTime = new TimeSpan(9, 0, 0)
		    }
	    };


	    logic.Setup(x => x.CreateAsync(It.IsAny<IEnumerable<IntervalDto>>()))
		    .ReturnsAsync(intervals);

	    List<IntervalToSendDto> dtos = new List<IntervalToSendDto>();
	    foreach (var i in intervals)
	    {
		    var newInterval = new IntervalToSendDto()
		    {
			    DayOfWeek = i.DayOfWeek,
			    EndTime = i.EndTime,
			    StartTime = i.StartTime
		    };
		    dtos.Add(newInterval);
	    }
		
	    // Act
	    var result = await _controller.CreateAsync(dtos);



        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
        CreatedResult createdResult = (CreatedResult)result.Result;
        Assert.AreEqual(intervals, createdResult.Value);
    }

    [TestMethod]
    public async Task CreateAsync_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        IEnumerable<IntervalDto> dto = new List<IntervalDto>(); // Set up a valid DTO.
        logic.Setup(x => x.CreateAsync(dto)).ThrowsAsync(new Exception("An error occurred."));

        IEnumerable<IntervalToSendDto> intervals = new List<IntervalToSendDto>();
        // Act
        var result = await _controller.CreateAsync(intervals);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        ObjectResult statusCodeResult = (ObjectResult)result.Result;
        Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }

    [TestMethod]
    public async Task GetAsync_AllSchedules_Test()
    {
	    // Arrange
	    var expected = new List<IntervalDto>
	    {
		    new IntervalDto
		    {
			    StartTime = new TimeSpan(9 , 0, 0),
			    EndTime = new TimeSpan(10, 0, 0)
		    },
		    new IntervalDto
		    {
			    StartTime = new TimeSpan(9 , 0, 0),
			    EndTime = new TimeSpan(10, 0, 0)
		    },
		    new IntervalDto
		    {
			    StartTime = new TimeSpan(9 , 0, 0),
			    EndTime = new TimeSpan(10, 0, 0)
		    },
	    };
	    logic.Setup(mock => mock.GetAsync()).ReturnsAsync(expected);


	    // Act
	    var response = await _controller.GetAsync();

	    // Assert
	    Assert.IsNotNull(response.Result);
	    Assert.IsInstanceOfType<OkObjectResult>(response.Result);

	    var okResult = response.Result as OkObjectResult;
	    Assert.AreEqual(expected, okResult.Value);

	    var actualIntervals = okResult.Value as IEnumerable<IntervalDto>;
	    Assert.IsNotNull(actualIntervals);
	    Assert.AreEqual(expected.Count, actualIntervals.Count());
	    Assert.AreEqual(200, okResult.StatusCode);
	    Assert.IsTrue(actualIntervals.SequenceEqual(expected));
    }
}
