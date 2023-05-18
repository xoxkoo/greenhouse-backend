// using Application.Logic;
// using Domain.DTOs;
// using Domain.DTOs.CreationDTOs;
// using EfcDataAccess.DAOs;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.VisualStudio.TestTools.UnitTesting;
// using SocketServer;
// using Testing.Utils;
// using WebAPI.Controllers;
//
// namespace Testing.IntegrationTests;
//
// [TestClass]
// public class PresetIntegrationTest : DbTestBase
// {
// 	private PresetController _controller;
//
// 	[TestInitialize]
// 	public void Initialize()
// 	{
// 		_controller = new PresetController(new PresetLogic(new PresetEfcDao(DbContext),new WebSocketServer(), new Converter(new)));
// 	}
//
// 	[TestMethod]
// 	public async Task UpdateAsync_ValidData_Test()
// 	{
// 		var preset = GetPreset();
// 		var creation = GetCreationPreset(preset);
//
// 		await _controller.CreateAsync(creation);
// 		await _controller.UpdateAsync(preset.Id, preset);
// 		var response = await _controller.GetAsync();
//
//
// 		Assert.IsNotNull(response);
// 		var createdResult = (ObjectResult?)response.Result;
// 		Assert.IsNotNull(createdResult);
// 		Assert.IsInstanceOfType(response.Result, typeof(OkObjectResult));
// 		Assert.AreEqual(200, ((OkObjectResult)response.Result).StatusCode);
//
// 		var result = (IEnumerable<PresetDto>?) createdResult.Value;
//
// 		Assert.AreEqual(preset.Name, result.FirstOrDefault().Name);
// 		Assert.AreEqual(preset.Id, result.FirstOrDefault().Id);
//
//
// 		// Assert the equality
// 		Assert.IsTrue(CompareThresholds(preset.Thresholds, result.FirstOrDefault().Thresholds));
//
// 	}
//
// 	private PresetEfcDto GetPreset()
// 	{
// 		var thresholds = new List<ThresholdDto>()
// 		{
// 			new ThresholdDto()
// 			{
// 				Type = "temperature",
// 				Min = 50,
// 				Max = 55
// 			},
// 			new ThresholdDto()
// 			{
// 				Type = "humidity",
// 				Min = 50,
// 				Max = 90
// 			},
// 			new ThresholdDto()
// 			{
// 				Type = "CO2",
// 				Min = 900,
// 				Max = 1200
// 			}
// 		};
// 		var preset = new PresetEfcDto()
// 		{
// 			Id = 1,
// 			Name = "Test",
// 			Thresholds = thresholds
// 		};
//
// 		return preset;
// 	}
//
// 	private PresetCreationDto GetCreationPreset(PresetEfcDto preset)
// 	{
// 		var creation = new PresetCreationDto()
// 		{
// 			Name = preset.Name,
// 			Thresholds = preset.Thresholds
// 		};
//
// 		foreach (var t in creation.Thresholds)
// 		{
// 			t.Max += 1;
// 			t.Min -= 1;
// 		}
//
// 		return creation;
// 	}
//
// 	private bool CompareThresholds(IEnumerable<ThresholdDto> thresholds1, IEnumerable<ThresholdDto> thresholds2)
// 	{
//
// 		foreach (var expected in thresholds1)
// 		{
// 			foreach (var actual in thresholds2)
// 			{
// 				if (expected.Type.Equals(actual.Type))
// 				{
// 					if (actual.Max != expected.Max) return false;
// 					if (actual.Min != expected.Min) return false;
// 				}
// 			}
// 		}
// 		return true;
// 	}
// }
