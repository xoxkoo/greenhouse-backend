using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Testing.LogicTests;

[TestClass]
public class ScheduleLogicTest
{
    public Mock<IScheduleDao> dao;
    private IScheduleLogic logic;
    private Mock<IConverter> converter;

    [TestInitialize]
    public void TestInitialize()
    {
	    converter = new Mock<IConverter>();
        dao = new Mock<IScheduleDao>();
        logic = new ScheduleLogic(dao.Object, converter.Object);
    }

    //CreateAsync() tests
    //Z - Zero
    [TestMethod]
    public async Task CreateSchedule_NullDto_Test()
    {
        //Arrange
        IEnumerable<IntervalDto> intervals = null;

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => logic.CreateAsync(intervals));
    }

    //O - One
    [TestMethod]
    public async Task SaveAsync_One_Test()
    {
        // Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>
        {
            new IntervalDto
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            }
        };
        

        dao
            .Setup(x => x.CreateAsync(It.IsAny<IEnumerable<Interval>>()))
            .ReturnsAsync(intervals);

        // Act
        var result = await logic.CreateAsync(intervals);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());

        var intervalDto = result.First();
        var interval = intervals.First();

        Assert.AreEqual(interval.DayOfWeek, intervalDto.DayOfWeek);
        Assert.AreEqual(interval.StartTime, intervalDto.StartTime);
        Assert.AreEqual(interval.EndTime, intervalDto.EndTime);
    }

    //M - Many
    [TestMethod]
    public async Task SaveAsync_Many_Test()
    {
        // Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>()
        {
            new IntervalDto()
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            },
            new IntervalDto()
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(18, 0, 0)
            }
        };
        
        dao
            .Setup(x => x.CreateAsync(It.IsAny<IEnumerable<Interval>>()))
            .ReturnsAsync(intervals);

        // Act
        var result = await logic.CreateAsync(intervals);

        // Assert
        Assert.AreEqual(intervals.Count(), result.Count());

        for (int i = 0; i < intervals.Count(); i++)
        {
            var expectedIntervals = intervals.OrderBy(x => x.StartTime).ToList();
            var actualIntervals = result.OrderBy(x => x.StartTime).ToList();

            Assert.AreEqual(expectedIntervals.Count, actualIntervals.Count);

            for (int j = 0; j < expectedIntervals.Count; j++)
            {
                Assert.AreEqual(expectedIntervals[j].DayOfWeek, actualIntervals[j].DayOfWeek);
                Assert.AreEqual(expectedIntervals[j].StartTime, actualIntervals[j].StartTime);
                Assert.AreEqual(expectedIntervals[j].EndTime, actualIntervals[j].EndTime);
            }
        }
    }
    //B - Boundaries

    //E - Exceptional behaviour
    [TestMethod]
    public async Task CreateSchedule_EmptyIntervals_Test()
    {
        //Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>();


        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => logic.CreateAsync(intervals));
    }

    [TestMethod]
    public async Task CreateSchedule_InvalidIntervalTimes_Test()
    {
        //Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>
        {
            new IntervalDto
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(7, 0, 0)
            }
        };

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => logic.CreateAsync(intervals));
    }

    [TestMethod]
    public async Task CreateSchedule_OverlappingIntervals_Test()
    {
        //Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>
        {
            new IntervalDto
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(9, 0, 0)
            },
            new IntervalDto
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(8, 30, 0),
                EndTime = new TimeSpan(10, 0, 0)
            }
        };

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => logic.CreateAsync(intervals));
    }


    //GetAsync() tests
    [TestMethod]
    public async Task TestGetSchedules_ReturnsSchedulesFromDao()
    {
        // Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>()
        {
            new IntervalDto()
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            },
            new IntervalDto()
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(18, 0, 0)
            }
        };
        dao.Setup(d => d.GetAsync()).ReturnsAsync(intervals);

        // Act
        var result = await logic.GetAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(intervals.Count(), result.Count());
        Assert.AreEqual(intervals.First().Id, result.First().Id);
    }
    //B - Boundaries

    // Test that a schedule with the earliest possible start time is saved correctly
    [TestMethod]
    public async Task CreateSchedule_EarliestStartTime_Test()
    {
        //Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>
        {
            new IntervalDto
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = TimeSpan.Zero,
                EndTime = new TimeSpan(1, 0, 0)
            }
        };
        dao
            .Setup(x => x.CreateAsync(It.IsAny<IEnumerable<Interval>>()))
            .ReturnsAsync(intervals);

        //Act
        var result = await logic.CreateAsync(intervals);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        var intervalDto = result.First();
        var interval = intervals.First();
        Assert.AreEqual(interval.DayOfWeek, intervalDto.DayOfWeek);
        Assert.AreEqual(interval.StartTime, intervalDto.StartTime);
        Assert.AreEqual(interval.EndTime, intervalDto.EndTime);
    }


    // Test that a schedule with the latest possible end time is saved correctly
    [TestMethod]
    public async Task CreateSchedule_LatestEndTime_Test()
    {
        //Arrange
        IEnumerable<IntervalDto> intervals = new List<IntervalDto>
        {
            new IntervalDto
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(23, 0, 0),
                EndTime = new TimeSpan(23, 59, 59)
            }
        };
        dao
            .Setup(x => x.CreateAsync(It.IsAny<IEnumerable<Interval>>()))
            .ReturnsAsync(intervals);

        //Act
        var result = await logic.CreateAsync(intervals);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        var intervalDto = result.First();
        var interval = intervals.First();
        Assert.AreEqual(interval.DayOfWeek, intervalDto.DayOfWeek);
        Assert.AreEqual(interval.StartTime, intervalDto.StartTime);
        Assert.AreEqual(interval.EndTime, intervalDto.EndTime);
    }
    
    [TestMethod]
    public async Task GetScheduleForDay_ReturnsCorrectData()
    {
        // Arrange
        var dayOfWeek = DayOfWeek.Monday;
        var expectedData = new List<IntervalToSendDto>
        {
            new IntervalToSendDto { StartTime =new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0) },
            new IntervalToSendDto { StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(14, 0, 0) }
        };

        dao.Setup(x => x.GetScheduleForDay(dayOfWeek)).ReturnsAsync(expectedData);

        // Act
        var actualData = await logic.GetScheduleForDay(dayOfWeek);

        // Assert
        CollectionAssert.AreEqual(expectedData, actualData.ToList());
    }

    [TestMethod]
    public async Task GetScheduleForDay_CallsCorrectDaoMethod()
    {
        // Arrange
        var dayOfWeek = DayOfWeek.Tuesday;

        // Act
        var actualData = await logic.GetScheduleForDay(dayOfWeek);

        // Assert
        dao.Verify(x => x.GetScheduleForDay(dayOfWeek), Times.Once);
    }
}
