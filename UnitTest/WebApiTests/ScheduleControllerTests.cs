using System.Diagnostics;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.DTOs.ScheduleDTOs;
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
	    var intervals = new List<IntervalDto>
	    {
		    new IntervalDto
		    {
			    DayOfWeek = DayOfWeek.Monday,
			    StartTime = new TimeSpan(8, 0, 0),
			    EndTime = new TimeSpan(9, 0, 0)
		    }
	    };

	    var createdDto = new ScheduleDto
	    {
		    Id = 1,
		    Intervals = intervals
	    };

	    logic.Setup(x => x.CreateAsync(It.IsAny<ScheduleCreationDto>()))
		    .ReturnsAsync(createdDto);

	    // Act
	    var result = await _controller.CreateAsync(intervals);



        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        OkObjectResult createdResult = (OkObjectResult)result.Result;
        var resultObj = createdResult.Value as ScheduleCreationDto;
        Assert.AreEqual(createdDto.Intervals.Count(), resultObj.Intervals.Count());
    }

    [TestMethod]
    public async Task CreateAsync_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        ScheduleCreationDto dto = new ScheduleCreationDto(); // Set up a valid DTO.
        logic.Setup(x => x.CreateAsync(dto)).ThrowsAsync(new Exception("An error occurred."));

        // Act
        ActionResult<ScheduleCreationDto> result = await _controller.CreateAsync(new List<IntervalDto>());

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
	    var intervalDtoList = expected;
	    var scheduleDto = new ScheduleDto
	    {
		    Intervals = intervalDtoList
	    };
	    var scheduleList = new List<ScheduleDto> { scheduleDto };
	    logic.Setup(mock => mock.GetAsync()).ReturnsAsync(scheduleList);


	    // Act
	    var response = await _controller.GetAsync();

	    // Assert
	    Assert.IsNotNull(response.Result);
	    Assert.IsInstanceOfType<OkObjectResult>(response.Result);

	    var okResult = response.Result as OkObjectResult;
	    Assert.AreEqual(intervalDtoList, okResult.Value);

	    var actualIntervals = okResult.Value as IEnumerable<IntervalDto>;
	    Assert.IsNotNull(actualIntervals);
	    Assert.AreEqual(expected.Count, actualIntervals.Count());
	    Assert.AreEqual(200, okResult.StatusCode);
	    Assert.IsTrue(actualIntervals.SequenceEqual(expected));
    }
}
