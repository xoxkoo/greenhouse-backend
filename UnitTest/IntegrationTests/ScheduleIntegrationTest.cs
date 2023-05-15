using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.DTOs.ScheduleDTOs;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.IntegrationTests;

[TestClass]
public class ScheduleIntegrationTest : DbTestBase
{
    private IScheduleDao dao;
    private IScheduleLogic logic;
    private Mock<IConverter> converterMock;

    [TestInitialize]
    public void TestInitialize()
    {
        converterMock = new Mock<IConverter>();
        dao = new ScheduleEfcDao(DbContext);
        logic = new ScheduleLogic(dao, converterMock.Object);
    }

    [TestMethod]
    public async Task CreateSchedule_Overlapping_Test()
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
                },
                new IntervalDto
                {
                DayOfWeek = DayOfWeek.Monday,
                StartTime = new TimeSpan(18, 0, 0),
                EndTime = new TimeSpan(21, 0, 0)
            }
            }
        };


        // Act
        var result = await logic.CreateAsync(dto);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Id);
        Assert.AreEqual(2, result.Intervals.Count());

        var intervalDto = result.Intervals.First();
        var interval = dto.Intervals.First();

        Assert.AreEqual(interval.DayOfWeek, intervalDto.DayOfWeek);
        Assert.AreEqual(interval.StartTime, intervalDto.StartTime);
        Assert.AreEqual(interval.EndTime, intervalDto.EndTime);
    }
}
