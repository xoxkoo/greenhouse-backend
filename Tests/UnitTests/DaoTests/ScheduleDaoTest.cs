using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Utils;

namespace Tests.UnitTests.DaoTests;

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
        IEnumerable<Interval> intervals = null;

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dao.CreateAsync(intervals));
    }

    //O - One
    [TestMethod]
    public async Task CreateSchedule_One_Test()
    {
        IEnumerable<Interval> intervals = new List<Interval>()
        {
            new Interval
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(12, 10, 0),
                EndTime = new TimeSpan(13, 10, 0)
            }
        };

        var createdSchedule = await dao.CreateAsync(intervals);
        Assert.IsNotNull(createdSchedule);
        Assert.AreEqual( intervals.Count(), createdSchedule.Count());
        Assert.AreEqual(intervals.FirstOrDefault()?.StartTime,
            createdSchedule.FirstOrDefault()?.StartTime);
        Assert.AreEqual(intervals.FirstOrDefault()?.EndTime,
            createdSchedule.FirstOrDefault()?.EndTime);
        Assert.AreEqual(intervals.FirstOrDefault()?.DayOfWeek,
            createdSchedule.FirstOrDefault()?.DayOfWeek);
    }

    //M - Many
    [TestMethod]
    public async Task CreateSchedule_Many_Test()
    {
        //Arrange
        IEnumerable<Interval> intervals = new List<Interval>
        {
            new Interval
            {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            },
            new Interval
            {
            DayOfWeek = DayOfWeek.Tuesday,
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(17, 0, 0)
        }
        };


        //Act
        var result = await dao.CreateAsync(intervals);


        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(intervals.Count(), result.Count());
        Assert.AreEqual(intervals.FirstOrDefault()!.DayOfWeek,
            result.FirstOrDefault()!.DayOfWeek);
        Assert.AreEqual(intervals.FirstOrDefault()!.StartTime, result.First().StartTime);
        Assert.AreEqual(intervals.FirstOrDefault()!.EndTime, result.First().EndTime);
        Assert.AreEqual(intervals.Count(), result.Count());
        Assert.AreEqual(intervals.FirstOrDefault()!.DayOfWeek,
            result.FirstOrDefault()!.DayOfWeek);
        Assert.AreEqual(intervals.FirstOrDefault()!.StartTime, result.First().StartTime);
        Assert.AreEqual(intervals.FirstOrDefault()!.EndTime, result.First().EndTime);
    }


    //B - Boundary
    //B - Boundary
    [TestMethod]
    public async Task CreateSchedule_Boundary_ValidInput_Test()
    {
        IEnumerable<Interval> intervals = new List<Interval>
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
        };
        
        var createdScheduleDto = await dao.CreateAsync(intervals);

        Assert.IsNotNull(createdScheduleDto);
        Assert.AreEqual(intervals.Count(), createdScheduleDto.Count());
        Assert.AreEqual(intervals.First().DayOfWeek, createdScheduleDto.First().DayOfWeek);
        Assert.AreEqual(intervals.First().StartTime, createdScheduleDto.First().StartTime);
        Assert.AreEqual(intervals.First().EndTime, createdScheduleDto.First().EndTime);
        Assert.AreEqual(intervals.Last().DayOfWeek, createdScheduleDto.Last().DayOfWeek);
        Assert.AreEqual(intervals.Last().StartTime, createdScheduleDto.Last().StartTime);
        Assert.AreEqual(intervals.Last().EndTime, createdScheduleDto.Last().EndTime);
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
        IEnumerable<Interval> intervals = new List<Interval>
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
        };
        DbContext.Intervals.AddRange(intervals);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await dao.GetAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(intervals.Count(), result.Count());
        foreach (var i in intervals)
        {
            var actualSchedule = result.SingleOrDefault(s => s.Id == i.Id);
            Assert.IsNotNull(actualSchedule);
        }
    }
    
    [TestMethod]
    public async Task GetScheduleForDay_ReturnsIntervalsForDayOfWeek()
    {
        // Arrange
        DayOfWeek testDayOfWeek = DayOfWeek.Monday;
        
        // Add some intervals to the database
        await DbContext.Intervals.AddRangeAsync(new List<Interval>
        {
            new Interval { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0) },
            new Interval { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(16, 0, 0) },
            new Interval { DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0) },
            new Interval { DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(16, 0, 0) },
        });
        await DbContext.SaveChangesAsync();

        // Act
        IEnumerable<IntervalToSendDto> result = await dao.GetScheduleForDay(testDayOfWeek);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());

        IntervalToSendDto firstInterval = result.ElementAt(0);
        Assert.AreEqual(new TimeSpan(10, 0, 0), firstInterval.StartTime);
        Assert.AreEqual(new TimeSpan(12, 0, 0), firstInterval.EndTime);

        IntervalToSendDto secondInterval = result.ElementAt(1);
        Assert.AreEqual(new TimeSpan(14, 0, 0), secondInterval.StartTime);
        Assert.AreEqual(new TimeSpan(16, 0, 0), secondInterval.EndTime);
    }
    
    

}