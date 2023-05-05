using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;
using WebAPI.Controllers;

namespace Testing.IntegrationTests.CO2IntegrationTests;

[TestClass]
public class CO2IntegrationTest : DbTestBase
{
    private CO2Controller _controller;
    private ICO2Dao _dao;
    private ICO2Logic _logic;
    
    [TestInitialize]
    public void Setup()
    {
        _dao = new CO2EfcDao(DbContext);
        _logic = new CO2Logic(_dao);
        _controller = new CO2Controller(_logic);
    }

    [TestMethod]
    public async Task CreateAsync_ValidData_Test()
    {
        // Arrange
        var co2CreateDto = new CO2CreateDto
        {
            Date = DateTime.Now,
            Value = 1000
        };

        // Act
        var result = await _controller.CreateAsync(co2CreateDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
        var createdResult = (CreatedResult)result.Result;
        Assert.AreEqual($"/co2s/1", createdResult.Location);
        Assert.AreEqual(201, createdResult.StatusCode);
        
        var checkResult = (CO2Dto?)createdResult.Value;
        Assert.AreEqual(1, checkResult.CO2Id);
        Assert.AreEqual(co2CreateDto.Date, checkResult.Date);
        Assert.AreEqual(co2CreateDto.Value, checkResult.Value);
    }
    
    [TestMethod]
    public async Task CreateAsync_InvalidCO2Dto_Test()
    {
        // Arrange
        var co2CreateDto = new CO2CreateDto
        {
            Date = DateTime.Now.AddDays(1),
            Value = -1
        };

        // Act
        ActionResult<CO2Dto> result = await _controller.CreateAsync(co2CreateDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        var statusCodeResult = (ObjectResult)result.Result;
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    
    [TestMethod]
    public async Task GetAsync_ReturnsOk_WithValidParameters_test()
    {
        // Arrange
        var co2 = new CO2
        {
            CO2Id = 1,
            Date = new DateTime(2023, 1, 2),
            Value = 1000
        };
        await DbContext.CO2s.AddAsync(co2);
        await DbContext.SaveChangesAsync();
        
        var startTime = new DateTime(2023, 1, 1);
        var endTime = new DateTime(2023, 1, 3);
        var current = false;

        // Act
        ActionResult<IEnumerable<CO2Dto>> response = await _controller.GetAsync(startTime, endTime, current);
        
        // Assert
        Assert.IsNotNull(response);
        var createdResult = (ObjectResult?)response.Result;
        Assert.IsNotNull(createdResult);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
        
        var result =(IEnumerable<CO2Dto>?) createdResult.Value;
        Assert.AreEqual(1, result.FirstOrDefault().CO2Id);
        Assert.AreEqual(co2.Value, result.FirstOrDefault().Value);
        Assert.AreEqual(co2.Date, result.FirstOrDefault().Date);
    }
    
    [TestMethod]
    public async Task GetAsync_ReturnsException_WithInvalidParameters_test()
    {
        // Arrange
        var co2 = new CO2
        {
            CO2Id = 1,
            Date = new DateTime(2023, 1, 2),
            Value = 1000
        };
        await DbContext.CO2s.AddAsync(co2);
        await DbContext.SaveChangesAsync();
        
        var startTime = new DateTime(2023, 1, 3);
        var endTime = new DateTime(2023, 1, 1);
        var current = false;

        // Act
        ActionResult<IEnumerable<CO2Dto>> response = await _controller.GetAsync(startTime, endTime, current);
        
        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(ObjectResult));
        var statusCodeResult = (ObjectResult)response.Result;
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    
}