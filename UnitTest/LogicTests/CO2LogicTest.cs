using System.Collections;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.WebApiTests;

[TestClass]
public class CO2LogicTest : DbTestBase
{
    private Mock<ICO2Dao> dao;
    private ICO2Logic logic;

    [TestInitialize]
    public void CO2LogicTestInit()
    {
        base.TestInit();
        dao = new Mock<ICO2Dao>();
        logic = new CO2Logic(dao.Object);
    }
    [TestMethod]
    public async Task CO2CreateAsyncTest()
    {
        dao.Setup(dao => dao.SaveAsync(It.IsAny<CO2>()))
            .ReturnsAsync(new CO2Dto { CO2Id = 1, Date = new DateTime(2023, 4, 19, 19, 50, 0), Value = 100 });
        CO2CreateDto dto = new CO2CreateDto
        {
            Date = new DateTime(2023, 4, 19, 19, 50, 0), 
            Value = 100 
        };
        CO2Dto created = await logic.CreateAsync(dto);
        Assert.AreEqual(dto.Date, created.Date);
        Assert.AreEqual(dto.Value, created.Value);
    }

    [TestMethod]
    public async Task CO2CreateAsyncIncorrectTest()
    {
        dao.Setup(dao => dao.SaveAsync(It.IsAny<CO2>()))
            .ReturnsAsync(new CO2Dto { CO2Id = 1, Date = new DateTime(2023, 4, 19, 19, 50, 0), Value = 100 });
        CO2CreateDto dto = new CO2CreateDto
        {
            Date = new DateTime(2023, 4, 19, 19, 50, 0), 
            Value = 1001
        };
        var expectedValues = "Wrong CO2 value, must be between 0 and 1000.";
        try
        {
            CO2Dto created = await logic.CreateAsync(dto);
        }
        catch (Exception e)
        {
            Assert.AreEqual(expectedValues, e.Message);
        }
    }
    [TestMethod]
    public async Task CO2GetAsyncCurrentTrueCorrectTest()
    {
        CO2Dto tempDto = new CO2Dto { CO2Id = 1, Date = new DateTime(2023, 4, 19, 19, 50, 0), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<CO2Dto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(true);
        IEnumerable<CO2Dto> co2s = await logic.GetAsync(search);
        Assert.IsNotNull(co2s.First());
        Assert.AreEqual(1, co2s.First().CO2Id);
        Assert.AreEqual(dao.Object.GetAsync(search).Result.First(), co2s.First());
    }
    [TestMethod]
    public async Task CO2GetAsyncCurrentTrueIncorrectDateTest()
    {
        CO2Dto tempDto = new CO2Dto { CO2Id = 1, Date = new DateTime(2023, 4, 19, 19, 50, 0), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<CO2Dto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(true, new DateTime(2024,04,5), new DateTime(2022,04,05));
        var expectedErrorMessage = "tart date cannot be before the end date";
        try
        {
            IEnumerable<CO2Dto> co2s = await logic.GetAsync(search);
        }
        catch (Exception e)
        {
            Assert.AreEqual(expectedErrorMessage, e.Message);
        }
    }
}