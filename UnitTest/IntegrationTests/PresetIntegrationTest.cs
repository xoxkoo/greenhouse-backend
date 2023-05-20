using Application.Logic;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocketServer;
using Testing.Utils;
using WebAPI.Controllers;

namespace Testing.IntegrationTests;

[TestClass]
public class PresetIntegrationTest : DbTestBase
{
	private PresetController _controller;

	[TestInitialize]
	public void Initialize()
	{
		_controller = new PresetController(new PresetLogic(new PresetEfcDao(DbContext),new WebSocketServer(), new Converter(new TemperatureLogic(new TemperatureEfcDao(DbContext)), new CO2Logic(new CO2EfcDao(DbContext)), new HumidityLogic(new HumidityEfcDao(DbContext)), new EmailLogic(new EmailEfcDao(DbContext), new PresetEfcDao(DbContext)))));
	}

	[TestMethod]
	public async Task CreateAsync_ValidData_Test()
	{
		var preset = GetPreset();
		var creation = GetCreationPreset(preset);

		var response = await _controller.CreateAsync(creation);

		Assert.IsNotNull(response);
		var createdResult = (CreatedResult?)response.Result;
		Assert.IsNotNull(createdResult);
		Assert.IsInstanceOfType(response.Result, typeof(CreatedResult));
		Assert.AreEqual($"/presets/1", createdResult.Location);
		Assert.AreEqual(201, createdResult.StatusCode);

		var result = (PresetEfcDto?) createdResult.Value;

		Assert.AreEqual(creation.Name, result.Name);
		Assert.AreEqual(1, result.Id);

		// Assert the equality
		Assert.IsTrue(CompareThresholds(creation.Thresholds, result.Thresholds));

	}

	[TestMethod]
	public async Task GetAsync_ValidData_Test()
	{

		var preset = GetPreset();
		var creation = GetCreationPreset(preset);

		for (int i = 0; i < 5; i++)
		{
			await _controller.CreateAsync(creation);
		}

		var response = await _controller.GetAsync();


		Assert.IsNotNull(response);
		var createdResult = (ObjectResult?)response.Result;
		Assert.IsNotNull(createdResult);
		Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
		Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);

		var result = (IEnumerable<PresetDto>?) createdResult.Value;

		Assert.AreEqual(5, result.Count());

		Assert.AreEqual(preset.Name, result.FirstOrDefault().Name);
		Assert.AreEqual(preset.Name, result.Last().Name);

		Assert.AreEqual(1, result.FirstOrDefault().Id);
		Assert.AreEqual(5, result.Last().Id);


		// Assert the equality
		Assert.IsTrue(CompareThresholds(preset.Thresholds, result.FirstOrDefault().Thresholds));
		Assert.IsTrue(CompareThresholds(preset.Thresholds, result.Last().Thresholds));

	}

	[TestMethod]
	public async Task UpdateAsync_ValidData_Test()
	{
		var preset = GetPreset();
		var creation = GetCreationPreset(preset);

		await _controller.CreateAsync(creation);
		await _controller.UpdateAsync(preset.Id, preset);
		var response = await _controller.GetAsync();


		Assert.IsNotNull(response);
		var createdResult = (ObjectResult?)response.Result;
		Assert.IsNotNull(createdResult);
		Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
		Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);

		var result = (IEnumerable<PresetDto>?) createdResult.Value;

		Assert.AreEqual(preset.Name, result.FirstOrDefault().Name);
		Assert.AreEqual(preset.Id, result.FirstOrDefault().Id);


		// Assert the equality
		Assert.IsTrue(CompareThresholds(preset.Thresholds, result.FirstOrDefault().Thresholds));

	}

	private PresetEfcDto GetPreset()
	{
		var thresholds = new List<ThresholdDto>()
		{
			new ThresholdDto()
			{
				Type = "temperature",
				Min = 50,
				Max = 55
			},
			new ThresholdDto()
			{
				Type = "humidity",
				Min = 50,
				Max = 90
			},
			new ThresholdDto()
			{
				Type = "CO2",
				Min = 900,
				Max = 1200
			}
		};
		var preset = new PresetEfcDto()
		{
			Id = 1,
			Name = "Test",
			Thresholds = thresholds
		};

		return preset;
	}

	private PresetCreationDto GetCreationPreset(PresetEfcDto preset)
	{
		var creation = new PresetCreationDto()
		{
			Name = preset.Name,
			Thresholds = preset.Thresholds
		};

		foreach (var t in creation.Thresholds)
		{
			t.Max += 1;
			t.Min -= 1;
		}

		return creation;
	}

	private bool CompareThresholds(IEnumerable<ThresholdDto> thresholds1, IEnumerable<ThresholdDto> thresholds2)
	{

		foreach (var expected in thresholds1)
		{
			foreach (var actual in thresholds2)
			{
				if (expected.Type.Equals(actual.Type))
				{
					if (actual.Max != expected.Max) return false;
					if (actual.Min != expected.Min) return false;
				}
			}
		}
		return true;
	}
}
