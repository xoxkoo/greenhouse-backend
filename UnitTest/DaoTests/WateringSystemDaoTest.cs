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
    [TestMethod]
    public async Task ValveIsAlreadyOpen()
    {
        //Arrange
        ValveState valve = new ValveState(){Toggle = true};
        var expectedErrorMessage = "The valve is already on";
        await DbContext.ValveState.AddAsync(valve);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dao.CreateAsync(valve), expectedErrorMessage);
    }
    [TestMethod]
    public async Task ValveIsAlreadyClosed()
    {
        //Arrange
        ValveState valve = new ValveState(){Toggle = true};
        var expectedErrorMessage = "The valve is already closed";
        await DbContext.ValveState.AddAsync(valve);

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await dao.CreateAsync(valve), expectedErrorMessage);
    }

}