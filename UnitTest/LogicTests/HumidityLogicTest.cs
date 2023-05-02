using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.WebApiTests;
[TestClass]
public class HumidityLogicTest : DbTestBase
{
    private Mock<IHumidityDao> dao;
    private IHumidityLogic logic;
    [TestInitialize]
    public void TemperatureLogicTestInit()
    {
        base.TestInit();
        dao = new Mock<IHumidityDao>();
        logic = new HumidityLogic(dao.Object);
    }
    [TestMethod]
    public async Task HumidityCreateAsyncTest()
    {
        //Arrange
        dao.Setup(dao => dao.CreateAsync(It.IsAny<Humidity>()))
            .ReturnsAsync(new HumidityDto { HumidityId = 1, Date = DateTime.Now, Value = 10 });

        var dto = new HumidityCreationDto()
        {
            Value = 10
        };

        //Act
        var createdHumidity = await logic.CreateAsync(dto);

        //Assert
        Assert.IsNotNull(createdHumidity);
        Assert.AreEqual(1, createdHumidity.HumidityId);
        Assert.AreEqual(dto.Value, createdHumidity.Value);
        Assert.IsTrue(createdHumidity.Date > DateTime.Now.AddSeconds(-1));
    }


    [TestMethod]
    public async Task HumidityGetAsyncTest()
    {
        //Arrange
        var searchMeasurementDto = new SearchMeasurementDto(true,  new DateTime(2023, 1, 1), new DateTime(2023, 4, 19));
        var humDto = new HumidityDto() { Date = new DateTime(2001, 1, 10), Value = 10, HumidityId = 1 };
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<HumidityDto>{humDto});

        //Act
        var humidities = await logic.GetAsync(searchMeasurementDto);

        // Assert
        Assert.IsNotNull(humidities);
        Assert.AreEqual(1, humidities.Count());
        Assert.AreEqual(humDto.HumidityId, humidities.First().HumidityId);
        Assert.AreEqual(humDto.Date, humidities.First().Date);
        Assert.AreEqual(humDto.Value, humidities.First().Value);
    }
    
    
    
    [TestMethod]
    public async Task HumidityGetAsyncCurrentTrueCorrectTest()
    {
        HumidityDto tempDto = new HumidityDto { HumidityId = 1, Date = new DateTime(2023, 4, 19, 19, 50, 0), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<HumidityDto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(true);
        IEnumerable<HumidityDto> temps = await logic.GetAsync(search);
        Assert.IsNotNull(temps.First());
        Assert.AreEqual(1, temps.First().HumidityId);
        Assert.AreEqual(dao.Object.GetAsync(search).Result.First(), temps.First());
    }
    [TestMethod]
    public async Task HumidityGetAsyncCurrentTrueIncorrectDateTest()
    {
        HumidityDto tempDto = new HumidityDto { HumidityId = 1, Date = new DateTime(2023, 4, 19, 19, 50, 0), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<HumidityDto>{tempDto});
        SearchMeasurementDto search = new SearchMeasurementDto(true, new DateTime(2024,04,5), new DateTime(2022,04,05));
        var expectedErrorMessage = "Start date cannot be before the end date";
        try
        {
            IEnumerable<HumidityDto> hums = await logic.GetAsync(search);
        }
        catch (Exception e)
        {
            Assert.AreEqual(expectedErrorMessage, e.Message);
        }
    }
    [TestMethod]
    public async Task HumidityGetAsyncCurrentTrue_EmptyDatabase()
    {
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<HumidityDto>());
        SearchMeasurementDto search = new SearchMeasurementDto(true, null, null);
        IEnumerable<HumidityDto> hums = await logic.GetAsync(search);
        Assert.AreEqual(0, hums.Count());
    }
    [TestMethod]
    public async Task HumidityGetAsyncCurrentFalseCorrectDateTest()
    {
        HumidityDto dto = new HumidityDto { HumidityId = 1, Date = new DateTime(2023, 4, 19, 19, 50, 0), Value = 100};
        HumidityDto dto1 = new HumidityDto { HumidityId = 2, Date = new DateTime(2023, 4, 20, 19, 50, 0), Value = 80};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<HumidityDto>{dto, dto1});
        SearchMeasurementDto search = new SearchMeasurementDto(false, new DateTime(2022,04,05),new DateTime(2024,04,5));
        IEnumerable<HumidityDto> hums = await logic.GetAsync(search);
        Assert.AreEqual(2, hums.Count());
    }
    [TestMethod]
    public async Task HumidityGetAsyncCurrentFalseCorrectDateTest2()
    {
        HumidityDto dto = new HumidityDto { HumidityId = 1, Date = new DateTime(2022, 3, 18, 19, 50, 0), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<HumidityDto>{dto});
        SearchMeasurementDto search = new SearchMeasurementDto(false, null,new DateTime(2024,04,5));
        IEnumerable<HumidityDto> hums = await logic.GetAsync(search);
        Assert.AreEqual(1, hums.Count());
        Assert.AreEqual(dto, hums.FirstOrDefault());
    }
    [TestMethod]
    public async Task HumidityGetAsyncCurrentFalseCorrectDateTest3()
    {
        HumidityDto dto = new HumidityDto { HumidityId = 1, Date = new DateTime(2022, 3, 18, 19, 50, 0), Value = 100};
        dao.Setup(dao => dao.GetAsync(It.IsAny<SearchMeasurementDto>()))
            .ReturnsAsync(new List<HumidityDto>{dto});
        SearchMeasurementDto search = new SearchMeasurementDto(false, new DateTime(2023, 04, 5), null);
        IEnumerable<HumidityDto> hums = await logic.GetAsync(search);
        Assert.AreEqual(1, hums.Count());
        Assert.AreEqual(dto, hums.FirstOrDefault());
    }

}