using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing.DaoTests;

[TestClass]
public class ScheduleDaoTest : DbTestBase
{
    private IScheduleDao dao;

    [TestInitialize]
    public void TestInitialize()
    {
        dao = new ScheduleEfcDao(DbContext);
    }

    //CreateAsync() tests
    //Z - Zero 
    [TestMethod]
    public async Task CreateSchedule_NullException_Test()
    {
        //Arrange 
        Schedule schedule = null;

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dao.CreateAsync(schedule));
    }

    //O - One 
    [TestMethod]
    public async Task CreateSchedule_One_Test()
    {
        var schedule = new Schedule
        {
            Intervals = new List<Interval>()
            {
                new Interval
                {
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(12, 10, 0),
                    EndTime = new TimeSpan(13, 10, 0)
                }
            }
        };

        var createdSchedule = await dao.CreateAsync(schedule);
        Console.WriteLine(createdSchedule.Intervals.FirstOrDefault()?.EndTime);
        Assert.IsNotNull(createdSchedule);
        Assert.AreEqual(schedule.Intervals.Count(), createdSchedule.Intervals.Count());
        Assert.AreEqual(schedule.Intervals.FirstOrDefault()?.StartTime,
            createdSchedule.Intervals.FirstOrDefault()?.StartTime);
        Assert.AreEqual(schedule.Intervals.FirstOrDefault()?.EndTime,
            createdSchedule.Intervals.FirstOrDefault()?.EndTime);
        Assert.AreEqual(schedule.Intervals.FirstOrDefault()?.DayOfWeek,
            createdSchedule.Intervals.FirstOrDefault()?.DayOfWeek);
    }

    //M - Many
    [TestMethod]
    public async Task CreateSchedule_Many_Test()
    {
        //Arrange
        var schedules = new List<Schedule>
        {
            new Schedule
            {
                Intervals = new List<Interval>
                {
                    new Interval
                    {
                        DayOfWeek = DayOfWeek.Monday,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0)
                    }
                }
            },
            new Schedule
            {
                Intervals = new List<Interval>
                {
                    new Interval
                    {
                        DayOfWeek = DayOfWeek.Tuesday,
                        StartTime = new TimeSpan(8, 30, 0),
                        EndTime = new TimeSpan(16, 30, 0)
                    }
                }
            },
            new Schedule
            {
                Intervals = new List<Interval>
                {
                    new Interval
                    {
                        DayOfWeek = DayOfWeek.Wednesday,
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0)
                    }
                }
            }
        };

        //Act
        var results = new List<ScheduleDto>();
        foreach (var sch in schedules)
        {
            var result = await dao.CreateAsync(sch);
            results.Add(result);
        }

        //Assert
        Assert.IsNotNull(results);
        Assert.AreEqual(3, results.Count);
        Assert.AreEqual(schedules[0].Intervals.Count(), results[0].Intervals.Count());
        Assert.AreEqual(schedules[0].Intervals.FirstOrDefault()!.DayOfWeek,
            results[0].Intervals.FirstOrDefault()!.DayOfWeek);
        Assert.AreEqual(schedules[0].Intervals.FirstOrDefault()!.StartTime, results[0].Intervals.First().StartTime);
        Assert.AreEqual(schedules[0].Intervals.FirstOrDefault()!.EndTime, results[0].Intervals.First().EndTime);
        Assert.AreEqual(schedules[1].Intervals.Count(), results[1].Intervals.Count());
        Assert.AreEqual(schedules[1].Intervals.FirstOrDefault()!.DayOfWeek,
            results[1].Intervals.FirstOrDefault()!.DayOfWeek);
        Assert.AreEqual(schedules[1].Intervals.FirstOrDefault()!.StartTime, results[1].Intervals.First().StartTime);
        Assert.AreEqual(schedules[1].Intervals.FirstOrDefault()!.EndTime, results[1].Intervals.First().EndTime);
    }
    
    
    //B - Boundary 
    //B - Boundary
    [TestMethod]
    public async Task CreateSchedule_Boundary_ValidInput_Test()
    {
        var schedule = new Schedule
        {
            Intervals = new List<Interval>
            {
                new Interval
                {
                    DayOfWeek = DayOfWeek.Monday, StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(12)
                },
                new Interval
                {
                    DayOfWeek = DayOfWeek.Tuesday, StartTime = TimeSpan.FromHours(10), EndTime = TimeSpan.FromHours(16)
                },
                new Interval
                {
                    DayOfWeek = DayOfWeek.Friday, StartTime = TimeSpan.FromHours(12), EndTime = TimeSpan.FromHours(18)
                },
            }
        };
        var createdScheduleDto = await dao.CreateAsync(schedule);

        Assert.IsNotNull(createdScheduleDto);
        Assert.IsNotNull(createdScheduleDto.Id);
        Assert.AreEqual(schedule.Intervals.Count(), createdScheduleDto.Intervals.Count());
        Assert.AreEqual(schedule.Intervals.First().DayOfWeek, createdScheduleDto.Intervals.First().DayOfWeek);
        Assert.AreEqual(schedule.Intervals.First().StartTime, createdScheduleDto.Intervals.First().StartTime);
        Assert.AreEqual(schedule.Intervals.First().EndTime, createdScheduleDto.Intervals.First().EndTime);
        Assert.AreEqual(schedule.Intervals.Last().DayOfWeek, createdScheduleDto.Intervals.Last().DayOfWeek);
        Assert.AreEqual(schedule.Intervals.Last().StartTime, createdScheduleDto.Intervals.Last().StartTime);
        Assert.AreEqual(schedule.Intervals.Last().EndTime, createdScheduleDto.Intervals.Last().EndTime);
    }
    
    //B + E - Boundary + Exceptional behavior
    [TestMethod]
    public async Task CreateSchedule_NullInput_Test()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dao.CreateAsync(null));
    }
    
    
    //GetAsync() tests
    //Z - Zero
    [TestMethod]
    public async Task TestGetAsync_ReturnsEmptyListWhenNoSchedules()
    {
        // Arrange

        // Act
        var result = await dao.GetAsync();

        // Assert
        Assert.AreEqual(0, result.Count());
    }
    
    //M - Many
    [TestMethod]
    public async Task GetAsync_Test_ReturnsAllSchedules()
    {
        // Arrange
        var expectedSchedules = new List<Schedule>
        {
            new Schedule { Id = 1, Intervals = new List<Interval>() },
            new Schedule { Id = 2, Intervals = new List<Interval>() },
            new Schedule { Id = 3, Intervals = new List<Interval>() }
        };
        DbContext.Schedules.AddRange(expectedSchedules);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await dao.GetAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedSchedules.Count(), result.Count());
        foreach (var expectedSchedule in expectedSchedules)
        {
            var actualSchedule = result.SingleOrDefault(s => s.Id == expectedSchedule.Id);
            Assert.IsNotNull(actualSchedule);
        }
    }
    
    [TestMethod]
    public async Task GetAsync_SchedulesWithIntervals_Test()
    {
        // Arrange
        var intervals = new List<IntervalDto>
        {
            new IntervalDto { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(12, 0, 0) },
            new IntervalDto { DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(18, 0, 0) }
        };
        var expectedSchedules = new List<ScheduleDto>
        {
            new ScheduleDto { Id = 1, Intervals = intervals },
            new ScheduleDto { Id = 2, Intervals = intervals }
        };
        DbContext.Schedules.AddRange(expectedSchedules.Select(s => new Schedule { Id = s.Id }));
        DbContext.Intervals.AddRange(intervals.Select(i => new Interval { DayOfWeek = i.DayOfWeek, StartTime = i.StartTime, EndTime = i.EndTime }));
        await DbContext.SaveChangesAsync();

        // Act
        var result = await dao.GetAsync();

        // Assert
        Assert.AreEqual(expectedSchedules.Count, result.Count());
        foreach (var expectedSchedule in expectedSchedules)
        {
            var actualSchedule = result.SingleOrDefault(s => s.Id == expectedSchedule.Id);
            Assert.IsNotNull(actualSchedule);
            Assert.AreEqual(expectedSchedule.Id, actualSchedule.Id);
        }
    }
    

}