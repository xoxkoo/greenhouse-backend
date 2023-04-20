using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.WebApiTests;

[TestClass]
public class CO2LogicTest : DbTestBase
{
    private readonly Mock<ICO2Logic> logic;

    public CO2LogicTest()
    {
        logic = new Mock<ICO2Logic>();
        logic
            .Setup(x => x.CreateAsync(It.IsAny<CO2CreateDto>()))
            .ReturnsAsync(new CO2Dto());
        
    }

    // [TestMethod]
    // public async Task CO2CreateAsyncTest()
    // {
    //     CO2CreateDto dto = new CO2CreateDto();
    //     dto.Date = new DateTime(1681977143);
    //     dto.Value = 10;
    //
    //     CO2Dto created = await logic.Object.CreateAsync(dto);
    //     Assert.AreEqual(created.Value,dto.Value);
    // }
}