using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;


namespace Testing.LogicTests;
[TestClass]
public class TemperatureLogicTest : DbTestBase
{
    private  Mock<ITemperatureDao> dao;
    private ITemperatureLogic logic;

    [TestInitialize]
    public void TemperatureLogicTestInit()
    {
        base.TestInit();
        dao = new Mock<ITemperatureDao>();
        logic = new TemperatureLogic(dao.Object);
    }
    [TestMethod]
    public async Task SaveAsyncTest()
    {
        //Arrange
        dao.Setup(dao => dao.CreateAsync(It.IsAny<Temperature>()))
            .ReturnsAsync(new TemperatureDto { TemperatureId = 1, Date = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds(), Value = 10 });

        var dto = new TemperatureCreateDto()
        {
            Value = 10
        };

        //Act
        var createdTemperature = await logic.CreateAsync(dto);

        //Assert
        Assert.IsNotNull(createdTemperature);
        Assert.AreEqual(1, createdTemperature.TemperatureId);
        Assert.AreEqual(dto.Value, createdTemperature.Value);
        Assert.IsTrue(createdTemperature.Date > ((DateTimeOffset)DateTime.Now.AddDays(-1)).ToUnixTimeSeconds());
    }


    [TestMethod]
    public async Task GetAsyncTest()
    {
        //Arrange
        var searchMeasurementDto = new SearchMeasurementDto(true,  new DateTime(2023, 1, 1), new DateTime(2023, 4, 19));
        var tempDto = new TemperatureDto { Date = ((DateTimeOffset)new DateTime(2001, 1, 10)).ToUnixTimeSeconds(), Value = 10, TemperatureId = 1 };
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<TemperatureDto>{tempDto});

        //Act
        var temperatures = await logic.GetAsync(searchMeasurementDto);

        // Assert
        Assert.IsNotNull(temperatures);
        Assert.AreEqual(1, temperatures.Count());
        Assert.AreEqual(tempDto.TemperatureId, temperatures.First().TemperatureId);
        Assert.AreEqual(tempDto.Date, temperatures.First().Date);
        Assert.AreEqual(tempDto.Value, temperatures.First().Value);
    }



    [TestMethod]
    public async Task CO2GetAsyncCurrentTrueCorrectTest()
    {
        TemperatureDto tempDto = new TemperatureDto { TemperatureId = 1, Date = ((DateTimeOffset)new DateTime(2023,4,19, 19, 50, 0)).ToUnixTimeSeconds(), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<TemperatureDto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(true);
        IEnumerable<TemperatureDto> temps = await logic.GetAsync(search);
        Assert.IsNotNull(temps.First());
        Assert.AreEqual(1, temps.First().TemperatureId);
        Assert.AreEqual(dao.Object.GetAsync(search).Result.First(), temps.First());
    }
    [TestMethod]
    public async Task CO2GetAsyncCurrentTrueIncorrectDateTest()
    {
        TemperatureDto tempDto = new TemperatureDto { TemperatureId = 1, Date = ((DateTimeOffset)new DateTime(2023,4,19, 19, 50, 0)).ToUnixTimeSeconds(), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<TemperatureDto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(true, new DateTime(2024,04,5), new DateTime(2022,04,05));
        var expectedErrorMessage = "Start date cannot be before the end date";
        try
        {
            IEnumerable<TemperatureDto> temps = await logic.GetAsync(search);
        }
        catch (Exception e)
        {
            Assert.AreEqual(expectedErrorMessage, e.Message);
        }
    }
    [TestMethod]
    public async Task CO2GetAsyncCurrentTrue_EmptyDatabase()
    {
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<TemperatureDto>());
        SearchMeasurementDto search = new SearchMeasurementDto(true, null, null);
        IEnumerable<TemperatureDto> temps = await logic.GetAsync(search);
        Assert.AreEqual(0, temps.Count());
    }
    [TestMethod]
    public async Task CO2GetAsyncCurrentFalseCorrectDateTest()
    {

        TemperatureDto tempDto = new TemperatureDto { TemperatureId = 1, Date = ((DateTimeOffset)new DateTime(2023,4,19, 19, 50, 0)).ToUnixTimeSeconds(), Value = 100};
        TemperatureDto tempDto1 = new TemperatureDto { TemperatureId = 2, Date = ((DateTimeOffset)new DateTime(2023,4,30, 19, 50, 0)).ToUnixTimeSeconds(), Value = 80};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<TemperatureDto>{tempDto, tempDto1});
        SearchMeasurementDto search = new SearchMeasurementDto(false, new DateTime(2022,04,05),new DateTime(2024,04,5));
        IEnumerable<TemperatureDto> temps = await logic.GetAsync(search);
        Assert.AreEqual(2, temps.Count());
    }
    [TestMethod]
    public async Task CO2GetAsyncCurrentFalseCorrectDateTest2()
    {
        TemperatureDto tempDto = new TemperatureDto { TemperatureId = 1, Date = ((DateTimeOffset)new DateTime(2022,3,18, 19, 50, 0)).ToUnixTimeSeconds(), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<TemperatureDto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(false, null,new DateTime(2024,04,5));
        IEnumerable<TemperatureDto> temps = await logic.GetAsync(search);
        Assert.AreEqual(1, temps.Count());
        Assert.AreEqual(tempDto, temps.FirstOrDefault());
    }
    [TestMethod]
    public async Task CO2GetAsyncCurrentFalseCorrectDateTest3()
    {
        TemperatureDto tempDto = new TemperatureDto { TemperatureId = 1, Date = ((DateTimeOffset)new DateTime(2022,3,18, 19, 50, 0)).ToUnixTimeSeconds(), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<TemperatureDto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(false, new DateTime(2023, 04, 5), null);
        IEnumerable<TemperatureDto> temps = await logic.GetAsync(search);
        Assert.AreEqual(1, temps.Count());
        Assert.AreEqual(tempDto, temps.FirstOrDefault());
    }
//B + E - Boundary + Exceptional behavior
    [TestMethod]
    public async Task CreateTemperature_AboveRange_Test()
    {
        var temperature = new TemperatureCreateDto()
        {
            Date = new DateTime(2023, 04, 23),
            Value = 61
        };

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => logic.CreateAsync(temperature));
    }

    [TestMethod]
    public async Task CreateTemperature_BelowRange_Test()
    {
        var temperature = new TemperatureCreateDto()
        {
            Date = new DateTime(2023, 04, 23),
            Value = -51
        };

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => logic.CreateAsync(temperature));
    }

    [TestMethod]
    public async Task CreateTemperature_FutureDate_Test()
    {
        var temperature = new TemperatureCreateDto()
        {
            Date = DateTime.Now.AddDays(1),
            Value = 25
        };

        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => logic.CreateAsync(temperature));
    }
}
