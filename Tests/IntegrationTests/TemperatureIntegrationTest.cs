using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Utils;
using WebAPI.Controllers;

namespace Tests.IntegrationTests;

[TestClass]
public class TemperatureIntegrationTest : DbTestBase
{
	private TemperatureController _controller;
	private ITemperatureDao _dao;
	private ITemperatureLogic _logic;

	[TestInitialize]
	public void Setup()
	{
		_dao = new TemperatureEfcDao(DbContext);
		_logic = new TemperatureLogic(_dao);
		_controller = new TemperatureController(_logic);
	}

	[TestMethod]
	public async Task CreateAsync_ValidData_Test()
	{
		TemperatureCreateDto dto = new TemperatureCreateDto()
		{
			Date = DateTime.Now,
			Value = (float)25.9
		};

		var response = await _logic.CreateAsync(dto);

		Assert.AreEqual(response.Value, dto.Value);

		var tmpDto = await _logic.GetAsync(new SearchMeasurementDto(true));

		Assert.AreEqual(tmpDto.FirstOrDefault().TemperatureId, response.TemperatureId);
		Assert.AreEqual(response.Value, tmpDto.FirstOrDefault().Value);

	}

	[TestMethod]
	public async Task GetAsync_GetCurrent_Test()
	{
		TemperatureCreateDto dto = new TemperatureCreateDto()
		{
			Date = DateTime.Now,
			Value = (float)25.9
		};

		await _logic.CreateAsync(dto);

		var result = await _controller.GetAsync(true);

		var createdResult = (ObjectResult?)result.Result;
		Assert.IsNotNull(createdResult);

		var list = (IEnumerable<TemperatureDto>?)createdResult.Value;
		Assert.IsNotNull(list);

		Assert.AreEqual(list.FirstOrDefault().TemperatureId, 1);
		Assert.AreEqual((float)25.9, list.FirstOrDefault().Value);

	}

	[TestMethod]
	public async Task GetAsync_GetInRange_Test()
	{
		// Arrange
		var temp1 = new Temperature()
		{
			TemperatureId = 1,
			Date = new DateTime(2023, 1, 2, 10, 30, 0),
			Value = 1000
		};
		var temp2 = new Temperature()
		{
			TemperatureId = 2,
			Date = new DateTime(2023, 1, 2, 10, 35, 0),
			Value = 1020
		};

		var temp3 = new Temperature()
		{
			TemperatureId = 3,
			Date = new DateTime(2023, 1, 2, 10, 38, 0),
			Value = 1020
		};

		await DbContext.Temperatures.AddAsync(temp1);
		await DbContext.Temperatures.AddAsync(temp2);
		await DbContext.Temperatures.AddAsync(temp3);
		await DbContext.SaveChangesAsync();

		var startTime = new DateTime(2023, 1, 2, 10, 29, 0);
		var endTime = new DateTime(2023, 1, 2, 10, 36, 10);

		// Act
		ActionResult<IEnumerable<TemperatureDto>> response = await _controller.GetAsync(false, startTime, endTime);
		// Assert
		Assert.IsNotNull(response);
		var createdResult = (ObjectResult?)response.Result;
		Assert.IsNotNull(createdResult);
		Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
		Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
		var result =(IEnumerable<TemperatureDto>?) createdResult.Value;
		Assert.AreEqual(2, result.Count());
	}

	[TestMethod]
	public async Task GetAsync_Boundaries_Test()
	{
		// Arrange
		var temp1 = new Temperature()
		{
			TemperatureId = 1,
			Date = new DateTime(2023, 1, 2, 10, 30, 0),
			Value = 1000
		};
		var temp2 = new Temperature()
		{
			TemperatureId = 2,
			Date = new DateTime(2023, 1, 2, 10, 35, 0),
			Value = 1020
		};

		var temp3 = new Temperature()
		{
			TemperatureId = 3,
			Date = new DateTime(2023, 1, 2, 10, 36, 0),
			Value = 1020
		};

		await DbContext.Temperatures.AddAsync(temp1);
		await DbContext.Temperatures.AddAsync(temp2);
		await DbContext.Temperatures.AddAsync(temp3);
		await DbContext.SaveChangesAsync();

		var startTime = new DateTime(2023, 1, 2, 10, 30, 0);
		var endTime = new DateTime(2023, 1, 2, 10, 36, 10);

		// Act
		ActionResult<IEnumerable<TemperatureDto>> response = await _controller.GetAsync(false, null, endTime);
		// Assert
		Assert.IsNotNull(response);
		var createdResult = (ObjectResult?)response.Result;
		Assert.IsNotNull(createdResult);
		Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
		Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
		var result =(IEnumerable<TemperatureDto>?) createdResult.Value;
		Assert.AreEqual(3, result.Count());
	}

	private async Task CreateTemperatures(int num)
	{
		for (int i = 0; i < num; i++)
		{
			TemperatureCreateDto dto = new TemperatureCreateDto()
			{
				Date = new DateTime(2023, 5, 7, 16, i+27, 0),
				Value = (float)20.5 + i
			};


			await _logic.CreateAsync(dto);
			// Debug statement
			Console.WriteLine($"Number of temperatures in database: {DbContext.Temperatures.Count()}");

			Console.WriteLine(DbContext.Temperatures.FirstOrDefault().TemperatureId);
		}
	}

	//M - Many
	[TestMethod]
	public async Task GetAsync_WithValidParameters_Many_Test()
	{
		// Arrange
		var humidity1 = new Temperature()
		{
			TemperatureId = 1,
			Date = new DateTime(2023, 1, 2, 10, 30, 0),
			Value = 1000
		};
		var humidity2 = new Temperature()
		{
			TemperatureId = 2,
			Date = new DateTime(2023, 1, 2, 10, 35, 0),
			Value = 1020
		};

		var humidity3 = new Temperature()
		{
			TemperatureId = 3,
			Date = new DateTime(2023, 1, 2, 10, 36, 0),
			Value = 1020
		};

		await DbContext.Temperatures.AddAsync(humidity1);
		await DbContext.Temperatures.AddAsync(humidity2);
		await DbContext.Temperatures.AddAsync(humidity3);
		await DbContext.SaveChangesAsync();

		var startTime = new DateTime(2023, 1, 2, 10, 30, 0);
		var endTime = new DateTime(2023, 1, 2, 10, 35, 10);

		// Act
		ActionResult<IEnumerable<TemperatureDto>> response = await _controller.GetAsync(false, startTime, endTime);

		// Assert
		Assert.IsNotNull(response);
		var createdResult = (ObjectResult?)response.Result;
		Assert.IsNotNull(createdResult);
		Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
		Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);

		var result =(IEnumerable<TemperatureDto>?) createdResult.Value;
		Assert.AreEqual(2, result.Count());
	}

}
