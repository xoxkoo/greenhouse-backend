using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;
using Exception = System.Exception;


namespace Tests.UnitTests.WebApiTests;

[TestClass]
public class HumidityControllerTests
{
	[TestMethod]
	public async Task GetAsync_StartDateAfterEndDate_ReturnsBadRequest()
	{
		var expectedErrorMessage = "Start date cannot be before the end date";
		// Arrange
		var logicMock = new Mock<IHumidityLogic>();
		logicMock
			.Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>()))
			.ThrowsAsync(new Exception("Start date cannot be before the end date"));

		var controller = new HumidityController(logicMock.Object);
		// Act
		try
		{
			await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(-1));
		}
		catch (Exception e)
		{
			// Check
			Assert.AreEqual(expectedErrorMessage,e.Message);
		}

	}
	[TestMethod]
	public async Task GetAsync_checkValue()
	{
		long time = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
		HumidityDto dto = new HumidityDto(){Date = time,HumidityId = 1,Value = 50};
		IEnumerable<HumidityDto> list = new[] { dto };
		// Arrange
		var logicMock = new Mock<IHumidityLogic>();
		logicMock
			.Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>())).ReturnsAsync(list);

		var controller = new HumidityController(logicMock.Object);
		// Act
		await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(+1));
		// Check
		Assert.AreEqual(50,dto.Value);

	}
	[TestMethod]
	public async Task GetAsync_Date()
	{
		long time = ((DateTimeOffset)new DateTime(2001,1,1)).ToUnixTimeSeconds();
		HumidityDto dto = new HumidityDto(){Date = time,HumidityId = 1,Value = 50};
		IEnumerable<HumidityDto> list = new[] { dto };
		// Arrange
		var logicMock = new Mock<IHumidityLogic>();
		logicMock
			.Setup(x => x.GetAsync(It.IsAny<SearchMeasurementDto>())).ReturnsAsync(list);

		var controller = new HumidityController(logicMock.Object);
		// Act
		await controller.GetAsync(current: true, startTime: DateTime.Now, endTime: DateTime.Now.AddDays(+1));
		// Check
		Assert.AreEqual(((DateTimeOffset)new DateTime(2001,1,1)).ToUnixTimeSeconds(),dto.Date);
	}
}