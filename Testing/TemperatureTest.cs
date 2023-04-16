using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing;

[TestClass]
public class TemperatureTest
{
    [TestMethod]
    public void GetTemperature()
    {
        //Initializing Logic class
        ITemperatureLogic logic = new TemperatureLogic(new TemperatureEfcDao(new Context()));


        //Initializing new object with incorrect dates
        SearchMeasurementDto dto = new SearchMeasurementDto(new DateTime(2004,7,2,10,2,3), new DateTime(2003,5,7,1,6,35),false);

        //Assert
        var expectedErrorMessage = "Start date cannot be before the end date";
        Task<Exception> ex = Assert.ThrowsExceptionAsync<Exception>(() => logic.GetAsync(dto));
        Assert.AreEqual(expectedErrorMessage, ex.Result.Message);
    }


}
