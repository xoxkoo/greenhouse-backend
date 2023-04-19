using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;


namespace Testing.WebApiTests;
[TestClass]
public class TemperatureControllerTest
{
    private readonly Mock<ITemperatureLogic> logic;
    private readonly TemperatureController controller;

    public TemperatureControllerTest()
    {
        logic = new Mock<ITemperatureLogic>();
        logic
        	.Setup(x => x.CreateAsync(It.IsAny<TemperatureCreateDto>()))
        	.ReturnsAsync(new TemperatureDto());

        controller = new TemperatureController(logic.Object);
    }
    [TestMethod]
    public async Task GetAsyncTest()
    {
        var dto = new TemperatureCreateDto()
        {
            Date = DateTime.Now,
            value = 10
        };
        var mockTemp = new TemperatureDto()
        {
            TemperatureId = 1,
            Date = DateTime.Now,
            value = 10
        };
        logic
            .Setup(x => x.CreateAsync(dto))
            .ReturnsAsync(mockTemp);
        
        // var response =
        await logic.Object.CreateAsync(dto);
        var response = controller.GetAsync(false);
        // var createdResult = response.Result as CreatedResult;
        // Assert.IsNotNull(createdResult);
        // var createdOrder = createdResult.Value as Order;
        // Assert.IsNotNull(createdOrder);
        // Assert.AreEqual(order, createdOrder);
        // Assert.AreEqual($"/orders/{order.Id}", createdResult.Location);
    }
}