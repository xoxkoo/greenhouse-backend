using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing.DaoTests;
[TestClass]
public class WateringSystemDaoTest : DbTestBase
{
    private IWateringSystemDao dao;

    [TestInitialize]
    public void TestInitialize()
    {
        dao = new WateringSystemDao(DbContext);
    }


    //TODO this should be handled differently
    // [TestMethod]
    // public async Task ValveIsAlreadyOpen()
    // {
    //     //Arrange
    //     ValveState valve = new ValveState { Toggle = true };
    //     await DbContext.ValveState.AddAsync(valve);
    //     await DbContext.SaveChangesAsync();
    //     var expectedErrorMessage = "The valve is already True";
    //
    //     //Act and Assert
    //     var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await dao.CreateAsync(valve));
    //     Assert.AreEqual(expectedErrorMessage, exception.Message);
    // }

    //TODO this should be handled differently
    // [TestMethod]
    // public async Task ValveIsAlreadyClosed()
    // {
    //     //Arrange
    //     ValveState valve = new ValveState { Toggle = false };
    //     await DbContext.ValveState.AddAsync(valve);
    //     await DbContext.SaveChangesAsync();
    //     var expectedErrorMessage = "The valve is already False";
    //
    //     //Act and Assert
    //     var exception = await Assert.ThrowsExceptionAsync<Exception>(async () => await dao.CreateAsync(valve));
    //     Assert.AreEqual(expectedErrorMessage, exception.Message);
    // }

}
