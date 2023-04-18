using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Bson;

namespace Testing;

[TestClass]
public class CO2UnitTest
{
    [TestMethod]
    public void GetCO2IncorrectNotCurrent()
    {
        //Initializing Logic class
        ICO2Logic logic = new CO2Logic(new CO2EfcDao(new Context()));

        
        //Initializing new object with incorrect dates
        SearchMeasurementDto dto = new SearchMeasurementDto(false, new DateTime(2004,7,2,10,2,3), new DateTime(2003,5,7,1,6,35));

        //Assert
        var expectedErrorMessage = "Start date cannot be before the end date";
        Task<Exception> ex = Assert.ThrowsExceptionAsync<Exception>(() => logic.GetAsync(dto));
        Assert.AreEqual(expectedErrorMessage, ex.Result.Message);
    }
    [TestMethod]
    public void GetCO2CorrectNotCurrent()
    {
        //Initializing Logic class
        ICO2Logic logic = new CO2Logic(new CO2EfcDao(new Context()));

        
        //Initializing new object with correct dates
        SearchMeasurementDto dto = new SearchMeasurementDto(false,new DateTime(2002,7,2,10,2,3), new DateTime(2004,5,7,1,6,35));

        //Assert
        Assert.IsNotNull(() => logic.GetAsync(dto));
    }
}