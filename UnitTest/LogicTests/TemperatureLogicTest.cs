using System.Collections;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;


namespace Testing.WebApiTests;

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
            .ReturnsAsync(new TemperatureDto { TemperatureId = 1, Date = DateTime.Now, Value = 10 });
  
        var dto = new TemperatureCreateDto()
        {
            value = 10
        };
        
        //Act
        var createdTemperature = await logic.CreateAsync(dto);
        
        //Assert
        Assert.IsNotNull(createdTemperature);
        Assert.AreEqual(1, createdTemperature.TemperatureId);
        Assert.AreEqual(dto.value, createdTemperature.Value);
        Assert.IsTrue(createdTemperature.Date > DateTime.Now.AddSeconds(-1));
    }


    [TestMethod]
    public async Task GetAsyncTest()
    {
        //Arrange
        var searchMeasurementDto = new SearchMeasurementDto(true,  new DateTime(2023, 1, 1), new DateTime(2023, 4, 19));
        var tempDto = new TemperatureDto { Date = new DateTime(2001, 1, 10), Value = 10, TemperatureId = 1 };
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


}