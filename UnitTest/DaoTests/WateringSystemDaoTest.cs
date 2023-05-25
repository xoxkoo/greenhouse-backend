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

}
