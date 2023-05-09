using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;
using WebAPI.Controllers;

namespace UnitTest.IntegrationTests;

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

		Assert.AreEqual(response.value, dto.Value);

		var tmpDto = await _logic.GetAsync(new SearchMeasurementDto(true));

		Assert.AreEqual(tmpDto.FirstOrDefault().TemperatureId, response.TemperatureId);
		Assert.AreEqual(response.value, tmpDto.FirstOrDefault().value);

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
		Assert.AreEqual((float)25.9, list.FirstOrDefault().value);

	}

	[TestMethod]
	public async Task GetAsync_GetInRange_Test()
	{
		await CreateTemperatures(10);

		var result = await _controller.GetAsync(false, new DateTime(2023, 5, 7, 10, 0, 0), new DateTime(2023, 5, 7, 20, 0, 0));

		var createdResult = (ObjectResult?)result.Result;
		Assert.IsNotNull(createdResult);

		var list = (IEnumerable<TemperatureDto>?)createdResult.Value;
		Assert.IsNotNull(list);
		Assert.AreEqual(list.Count(), 10);
	}

	[TestMethod]
	public async Task GetAsync_Boundaries_Test()
	{
		await CreateTemperatures(10);

		// minutes are 0 and 2, so it should return 3 temperatures (0, 1, 2)
		var result = await _controller.GetAsync(false, new DateTime(2023, 5, 7, 16, 0, 0), new DateTime(2023, 5, 7, 16, 2, 0));

		var createdResult = (ObjectResult?)result.Result;
		Assert.IsNotNull(createdResult);

		var list = (IEnumerable<TemperatureDto>?)createdResult.Value;
		Assert.IsNotNull(list);
		Assert.AreEqual(list.Count(), 3);
	}

	private async Task CreateTemperatures(int num)
	{
		for (int i = 0; i < num; i++)
		{
			TemperatureCreateDto dto = new TemperatureCreateDto()
			{
				Date = new DateTime(2023, 5, 7, 16, i, 0),
				Value = (float)20.5 + i
			};

			await _logic.CreateAsync(dto);
		}
	}

}
