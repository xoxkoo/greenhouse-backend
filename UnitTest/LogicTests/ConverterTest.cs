﻿using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.WebApiTests;

[TestClass]
public class ConverterTest : DbTestBase
{

	private readonly Mock<ITemperatureLogic> tempLogic;
	private readonly Mock<IHumidityLogic> humidityLogic;
	private readonly Mock<ICO2Logic> co2logic;
	private readonly IConverter converter;


    public ConverterTest()
    {
	    tempLogic = new Mock<ITemperatureLogic>();
	    co2logic = new Mock<ICO2Logic>();
	    humidityLogic = new Mock<IHumidityLogic>();
        converter = new Converter(tempLogic.Object, co2logic.Object, humidityLogic.Object);
    }

    [TestMethod]
    public async Task THCPayload_SavedToDB()
    {
	    await converter.ConvertFromHex("07817b1f4ff0");

        // Assert
        tempLogic.Verify(x => x.CreateAsync(It.Is<TemperatureCreateDto>(dto =>
	        // tolerance because of rounding problems
	        Math.Abs(dto.value - 25.8) < 0.1
        )), Times.Once);

        co2logic.Verify(x => x.CreateAsync(It.Is<CO2CreateDto>(dto =>
	        dto.Value == 1279
        )), Times.Once);

        humidityLogic.Verify(x => x.CreateAsync(It.Is<HumidityCreationDto>(dto =>
	        dto.Value == 31
        )), Times.Once);
    }

    [TestMethod]
    public async Task THCPayload_ResponseStringIsCorrect()
    {
	    string result = await converter.ConvertFromHex("07817b1f4ff0");

	    Assert.AreEqual("25.8, 31, 1279", result);
    }

    [TestMethod]
    public void THCPayload_ThrowErrorWhenNotHexValue()
    {
	    Assert.ThrowsExceptionAsync<Exception>(() => converter.ConvertFromHex("t7800c9401e0"));
    }

    [TestMethod]
    public async Task THCPayload_IncorrectValue()
    {
	    Assert.ThrowsExceptionAsync<Exception>(() => converter.ConvertFromHex(""));
	    Assert.ThrowsExceptionAsync<Exception>(() => converter.ConvertFromHex("    "));
    }

}
