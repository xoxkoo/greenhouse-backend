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
    public async Task CreateAsync_ToTrue()
    {
	    const bool newState = true;
	    var r = await dao.CreateAsync(new ValveState(){Toggle = newState});

	    Assert.AreEqual(r.State, newState);
    }

    [TestMethod]
    public async Task CreateAsync_ToTrue_ToFalse()
    {
	    const bool newState = false;

	    await dao.CreateAsync(new ValveState(){Toggle = true});
	    var r = await dao.CreateAsync(new ValveState(){Toggle = newState});

	    Assert.AreEqual(r.State, newState);
    }

}
