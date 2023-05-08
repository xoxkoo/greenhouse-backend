using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;
using WebAPI.Controllers;

namespace Testing.IntegrationTests.CO2IntegrationTests;

[TestClass]
public class HumidityIntegrationTest : DbTestBase
{
    private HumidityController _controller;
    private IHumidityDao _dao;
    private IHumidityLogic _logic;
    
    [TestInitialize]
    public void Setup()
    {
        _dao = new HumidityEfcDao(DbContext);
        _logic = new HumidityLogic(_dao);
        _controller = new HumidityController(_logic);
    }

    //Z - Zero
    [TestMethod]
    public async Task GetAsync_Zero_Test()
    {
        // Arrange
        var current = true;

        // Act
        ActionResult<IEnumerable<HumidityDto>> response = await _controller.GetAsync(current);
        
        // Assert
        Assert.IsNotNull(response);
        var createdResult = (ObjectResult?)response.Result;
        Assert.IsNotNull(createdResult);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
        
        var result =(IEnumerable<HumidityDto>?) createdResult.Value;
        Assert.AreEqual(0, result.Count());
    }
    
    //O - One
    [TestMethod]
    public async Task GetAsync_WithValidParameters_test()
    {
        // Arrange
        var humidity = new Humidity()
        {
            HumidityId = 1,
            Date = new DateTime(2023, 1, 2),
            Value = 1000
        };
        await DbContext.Humidities.AddAsync(humidity);
        await DbContext.SaveChangesAsync();
        
        var startTime = new DateTime(2023, 1, 1);
        var endTime = new DateTime(2023, 1, 3);
        var current = false;

        // Act
        ActionResult<IEnumerable<HumidityDto>> response = await _controller.GetAsync(current, startTime, endTime);
        
        // Assert
        Assert.IsNotNull(response);
        var createdResult = (ObjectResult?)response.Result;
        Assert.IsNotNull(createdResult);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
        
        var result =(IEnumerable<HumidityDto>?) createdResult.Value;
        Assert.AreEqual(1, result.FirstOrDefault().HumidityId);
        Assert.AreEqual(humidity.Value, result.FirstOrDefault().Value);
        Assert.AreEqual(humidity.Date, result.FirstOrDefault().Date);
    }

    //M - Many
    [TestMethod]
    public async Task GetAsync_WithValidParameters_Many_Test()
    {
        // Arrange
        var humidity1 = new Humidity()
        {
            HumidityId = 1,
            Date = new DateTime(2023, 1, 2, 10, 30, 0),
            Value = 1000
        };
        var humidity2 = new Humidity()
        {
            HumidityId = 2,
            Date = new DateTime(2023, 1, 2, 10, 35, 0),
            Value = 1020
        };
        
        await DbContext.Humidities.AddAsync(humidity1);
        await DbContext.Humidities.AddAsync(humidity2);
        await DbContext.SaveChangesAsync();
        
        var startTime = new DateTime(2023, 1, 2, 10, 30, 0);
        var endTime = new DateTime(2023, 1, 2, 10, 35, 10);
        var current = false;

        // Act
        ActionResult<IEnumerable<HumidityDto>> response = await _controller.GetAsync(current, startTime, endTime);
        
        // Assert
        Assert.IsNotNull(response);
        var createdResult = (ObjectResult?)response.Result;
        Assert.IsNotNull(createdResult);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
        
        var result =(IEnumerable<HumidityDto>?) createdResult.Value;
        Assert.AreEqual(2, result.Count());
    }
        
    [TestMethod]
    public async Task GetAsync_Boundaries_Test() 
    {
        await CreateTemperatures(10);
        // minutes are 0 and 2, so it should return 3 temperatures (0, 1, 2)
        var result = await _controller.GetAsync(false, new DateTime(2023, 5, 7, 16, 0, 0), new DateTime(2023, 5, 7, 16, 2, 0));
        var createdResult = (ObjectResult?)result.Result; 
        Assert.IsNotNull(createdResult);
        var list = (IEnumerable<HumidityDto>?)createdResult.Value; 
        Assert.IsNotNull(list); 
        Assert.AreEqual(list.Count(), 3);
        }
    
        private async Task CreateTemperatures(int num)
        {
            for (int i = 0; i < num; i++)
            {
                HumidityCreationDto dto = new HumidityCreationDto()
                {
                    Date = new DateTime(2023, 5, 7, 16, i, 0),
                    Value = 1000 + i
                };
                
                await _logic.CreateAsync(dto);
                Console.WriteLine(DbContext.Humidities.FirstOrDefault().HumidityId);
            }
        }
        
        
    //B - Boundary
    [TestMethod]
    public async Task GetAsync_WithValidParameters_Boundaries_Test()
    {
        // Arrange
        var humidity = new Humidity()
        {
            HumidityId = 1,
            Date = new DateTime(2023, 1, 2, 10, 30, 0),
            Value = 1000
        };
        
        await DbContext.Humidities.AddAsync(humidity);
        await DbContext.SaveChangesAsync();
        
        var startTime = new DateTime(2023, 1, 2, 10, 30, 0);
        var endTime = new DateTime(2023, 1, 2, 10, 30, 0);
        var current = false;

        // Act
        ActionResult<IEnumerable<HumidityDto>> response = await _controller.GetAsync(current, startTime, endTime);
        
        // Assert
        Assert.IsNotNull(response);
        var createdResult = (ObjectResult?)response.Result;
        Assert.IsNotNull(createdResult);
        Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
        Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
        
        var result =(IEnumerable<HumidityDto>?) createdResult.Value;
        Assert.AreEqual(1, result.FirstOrDefault().HumidityId);
        Assert.AreEqual(humidity.Value, result.FirstOrDefault().Value);
        Assert.AreEqual(humidity.Date, result.FirstOrDefault().Date);
    }
    
    //E - Exception
    [TestMethod]
    public async Task GetAsync_ReturnsException_WithInvalidParameters_test()
    {
        // Arrange
        var humidity = new Humidity()
        {
            HumidityId = 1,
            Date = new DateTime(2023, 1, 2),
            Value = 1000
        };
        await DbContext.Humidities.AddAsync(humidity);
        await DbContext.SaveChangesAsync();
        
        var startTime = new DateTime(2023, 1, 3);
        var endTime = new DateTime(2023, 1, 1);
        var current = false;

        // Act
        ActionResult<IEnumerable<HumidityDto>> response = await _controller.GetAsync(current, startTime, endTime);
        
        // Assert
        Assert.IsNotNull(response);
        Assert.IsInstanceOfType(response.Result, typeof(ObjectResult));
        var statusCodeResult = (ObjectResult)response.Result;
        Assert.AreEqual(500, statusCodeResult.StatusCode);
    }
    
    
    

}