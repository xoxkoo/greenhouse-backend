using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;



namespace Testing.WebApiTests;

[TestClass]
public class TemperatureLogicTest
{
    private readonly Mock<ITemperatureLogic> logic;

    public TemperatureLogicTest()
    {
        logic = new Mock<ITemperatureLogic>();
        logic
            .Setup(x => x.CreateAsync(It.IsAny<TemperatureCreateDto>()))
            .ReturnsAsync(new TemperatureDto());
        
    }
    [TestMethod]
    public async Task SaveAsyncTest()
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
        var response = logic.Object.CreateAsync(dto);
        Console.WriteLine(response.Result.value);
    }
}