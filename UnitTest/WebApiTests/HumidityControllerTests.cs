using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;
using Xunit;
using Assert = Xunit.Assert;


namespace Testing.WebApiTests;

[TestClass]
public class HumidityControllerTests
{
	private readonly Mock<IHumidityLogic> logic;
	private readonly HumidityController controller;

	public HumidityControllerTests()
	{
		logic = new Mock<IHumidityLogic>();
		// logic
		// 	.Setup(x => x.GetAsync(new SearchMeasurementDto(true, null, null)))
		// 	.ReturnsAsync();

		controller = new HumidityController(logic.Object);
	}

	[TestMethod]
	public async Task GetAsyncTest()
	{
		var dto = new HumidityDto
		{
			Date = DateTime.Now,
			HumidityId = 1,
			Value = 10
		};

		logic
			.Setup(x => x.GetAsync(new SearchMeasurementDto(true, null, null)))
			.ReturnsAsync(new List<HumidityDto>{dto});

		var response = await controller.GetAsync(false);

		var result = Assert.IsType<OkObjectResult>(response.Result);
		var humidity = Assert.IsAssignableFrom<IEnumerable<HumidityDto>>(result.Value);

		//var firstHumidity = humidity.FirstOrDefault();


		//Console.WriteLine(humidity.FirstOrDefault().HumidityId);

		// Assert.NotNull(firstHumidity);
		// Assert.Equal(dto.Date, firstHumidity.Date);
		// Assert.Equal(dto.HumidityId, firstHumidity.HumidityId);
		// Assert.Equal(dto.Value, firstHumidity.Value);
	}
}
