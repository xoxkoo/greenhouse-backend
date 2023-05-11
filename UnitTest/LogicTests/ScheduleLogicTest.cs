using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.DTOs.ScheduleDTOs;
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
        ScheduleCreationDto dto = null;

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => logic.CreateAsync(dto));
    }

    //O - One
    [TestMethod]
    public async Task SaveAsync_One_Test()
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
                    EndTime = new TimeSpan(17, 0, 0)
                }
            }
        };

        var expectedSchedule = new ScheduleDto()
        {
            Id = 1,
            Intervals = new List<IntervalDto>
            {
                new IntervalDto()
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0)
                }
            }
        };

        dao
            .Setup(x => x.CreateAsync(It.IsAny<Schedule>()))
            .ReturnsAsync(expectedSchedule);

        // Act
        var result = await logic.CreateAsync(dto);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual(1, result.Intervals.Count());

        var intervalDto = result.Intervals.First();
        var interval = dto.Intervals.First();

        Assert.AreEqual(interval.DayOfWeek, intervalDto.DayOfWeek);
        Assert.AreEqual(interval.StartTime, intervalDto.StartTime);
        Assert.AreEqual(interval.EndTime, intervalDto.EndTime);
    }

    //M - Many
    [TestMethod]
    public async Task SaveAsync_Many_Test()
    {
        // Arrange
        var schedules = new List<ScheduleCreationDto>()
        {
            new ScheduleCreationDto()
            {
                Intervals = new List<IntervalDto>()
                {
                    new IntervalDto()
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(10, 0, 0)
                    }
                }
            },
            new ScheduleCreationDto()
            {
                Intervals = new List<IntervalDto>()
                {
                    new IntervalDto()
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeSpan(17, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0)
                    }
                }
            },
        };

        List<ScheduleCreationDto> expectedSchedule = new List<ScheduleCreationDto>
        {
            new ScheduleCreationDto
            {
                Intervals = new List<IntervalDto>
                {
                    new IntervalDto
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(10, 0, 0)
                    }
                }
            },
            new ScheduleCreationDto
            {
                Intervals = new List<IntervalDto>
                {
                    new IntervalDto
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeSpan(17, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0)
                    }
                }
            }
        };

        dao
            .Setup(x => x.CreateAsync(It.IsAny<Schedule>()))
            .ReturnsAsync((Schedule s) => new ScheduleDto
            {
                Id = s.Id,
                Intervals = s.Intervals.Select(i => new IntervalDto
                {
                    DayOfWeek = i.DayOfWeek,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime
                }).ToList()
            });

        // Act
        List<ScheduleDto> results = new List<ScheduleDto>();
        foreach (var schedule in schedules)
        {
            var result = await logic.CreateAsync(schedule);
            results.Add(result);
        }

        // Assert
        Assert.AreEqual(expectedSchedule.Count, results.Count);

        for (int i = 0; i < expectedSchedule.Count; i++)
        {
            var expectedIntervals = expectedSchedule[i].Intervals.OrderBy(x => x.StartTime).ToList();
            var actualIntervals = results[i].Intervals.OrderBy(x => x.StartTime).ToList();

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
        ScheduleCreationDto dto = new ScheduleCreationDto
        {
            Intervals = new List<IntervalDto>()
        };

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => logic.CreateAsync(dto));
    }

    [TestMethod]
    public async Task CreateSchedule_InvalidIntervalTimes_Test()
    {
        //Arrange
        ScheduleCreationDto dto = new ScheduleCreationDto
        {
            Intervals = new List<IntervalDto>
            {
                new IntervalDto
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(7, 0, 0)
                }
            }
        };

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => logic.CreateAsync(dto));
    }

    [TestMethod]
    public async Task CreateSchedule_OverlappingIntervals_Test()
    {
        //Arrange
        ScheduleCreationDto dto = new ScheduleCreationDto
        {
            Intervals = new List<IntervalDto>
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
            }
        };

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => logic.CreateAsync(dto));
    }


    //GetAsync() tests
    [TestMethod]
    public async Task TestGetSchedules_ReturnsSchedulesFromDao()
    {
        // Arrange
        var expectedSchedules = new List<ScheduleDto>
        {
            new ScheduleDto { Id = 1, Intervals = new List<IntervalDto>() },
            new ScheduleDto { Id = 2, Intervals = new List<IntervalDto>() },
            new ScheduleDto { Id = 3, Intervals = new List<IntervalDto>() }
        };
        dao.Setup(d => d.GetAsync()).ReturnsAsync(expectedSchedules);

        // Act
        var result = await logic.GetAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedSchedules.Count(), result.Count());
        Assert.AreEqual(expectedSchedules.First().Id, result.First().Id);
        Assert.AreEqual(expectedSchedules.First().Intervals.Count(), result.First().Intervals.Count());
    }
    //B - Boundaries

    // Test that a schedule with the earliest possible start time is saved correctly
    [TestMethod]
    public async Task CreateSchedule_EarliestStartTime_Test()
    {
        //Arrange
        var dto = new ScheduleCreationDto
        {
            Intervals = new List<IntervalDto>
            {
                new IntervalDto
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = TimeSpan.Zero,
                    EndTime = new TimeSpan(1, 0, 0)
                }
            }
        };
        dao
            .Setup(x => x.CreateAsync(It.IsAny<Schedule>()))
            .ReturnsAsync((Schedule s) => new ScheduleDto
            {
                Id = s.Id,
                Intervals = s.Intervals.Select(i => new IntervalDto
                {
                    DayOfWeek = i.DayOfWeek,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime
                }).ToList()
            });

        //Act
        var result = await logic.CreateAsync(dto);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Intervals.Count());
        var intervalDto = result.Intervals.First();
        var interval = dto.Intervals.First();
        Assert.AreEqual(interval.DayOfWeek, intervalDto.DayOfWeek);
        Assert.AreEqual(interval.StartTime, intervalDto.StartTime);
        Assert.AreEqual(interval.EndTime, intervalDto.EndTime);
    }


    // Test that a schedule with the latest possible end time is saved correctly
    [TestMethod]
    public async Task CreateSchedule_LatestEndTime_Test()
    {
        //Arrange
        var dto = new ScheduleCreationDto
        {
            Intervals = new List<IntervalDto>
            {
                new IntervalDto
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(23, 0, 0),
                    EndTime = new TimeSpan(23, 59, 59)
                }
            }
        };
        dao
            .Setup(x => x.CreateAsync(It.IsAny<Schedule>()))
            .ReturnsAsync((Schedule s) => new ScheduleDto
            {
                Id = s.Id,
                Intervals = s.Intervals.Select(i => new IntervalDto
                {
                    DayOfWeek = i.DayOfWeek,
                    StartTime = i.StartTime,
                    EndTime = i.EndTime
                }).ToList()
            });

        //Act
        var result = await logic.CreateAsync(dto);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Intervals.Count());
        var intervalDto = result.Intervals.First();
        var interval = dto.Intervals.First();
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
