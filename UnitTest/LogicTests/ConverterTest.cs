using Application.Logic;
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

        // converter.Setup(x => x.ConvertFromHex(It.IsAny<String>()));
    }

    [TestMethod]
    public void THCPayloadRead()
    {

	    // tempLogic.Setup(x => x.CreateAsync(It.IsAny<TemperatureCreateDto>()))
		   //  ;
	    // co2logic.Setup(x => x.CreateAsync(It.IsAny<CO2CreateDto>()))
		   //  ;
	    // humidityLogic.Setup(x => x.CreateAsync(It.IsAny<HumidityCreationDto>()))
		   //  ;

	    // converter.Setup(x => x.ConvertFromHex("07800c9401e0"));
        converter.ConvertFromHex("07800c9401e0");

        // Assert
        tempLogic.Verify(x => x.CreateAsync(It.Is<TemperatureCreateDto>(dto =>
	        dto.value == 25
        )), Times.Once);

        co2logic.Verify(x => x.CreateAsync(It.Is<CO2CreateDto>(dto =>
	        dto.Value == 30
        )), Times.Once);

        humidityLogic.Verify(x => x.CreateAsync(It.Is<HumidityCreationDto>(dto =>
	        dto.Value == 20
        )), Times.Once);
        // var response = tempLogic
	       //  .Setup(x => x.GetAsync(new SearchMeasurementDto(false, null, null)))
	       //  .ReturnsAsync(list);
        // Console.WriteLine(response);
        // // int temp =
    }
}
