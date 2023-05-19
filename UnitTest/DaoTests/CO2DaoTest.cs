using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing.DaoTests;

[TestClass]
public class CO2DaoTest : DbTestBase
{
    private ICO2Dao dao;

    [TestInitialize]
    public void TestInitialize()
    {
        dao = new CO2EfcDao(DbContext);
    }

    [TestMethod]
    public async Task SaveCO2_NullException_Test()
    {
        //Arrange
        CO2 co2 = null;

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dao.CreateAsync(co2));
    }

    //O - One
    [TestMethod]
    public async Task SaveCO2_Test()
    {
        var co2 = new CO2
        {
            Date = new DateTime(2023, 4, 10),
            Value = 1000
        };

        var savedCO2 = await dao.CreateAsync(co2);

        Assert.IsNotNull(savedCO2);
        Assert.AreEqual(1, savedCO2.CO2Id);
        Assert.AreEqual(co2.Value, savedCO2.Value);
        Assert.AreEqual(co2.Date, savedCO2.Date);
    }

    //M - Many
    [TestMethod]
    public async Task SaveCO2_Many_Test()
    {
        //Arrange
        var co2s = new List<CO2>
        {
            new CO2
            {
                Date = new DateTime(2023, 04, 22),
                Value = 2000
            },
            new CO2
            {
                Date = new DateTime(2023, 04, 23),
                Value = 1500
            },
            new CO2
            {
                Date = new DateTime(2023, 04, 21),
                Value = 1750
            }
        };

        //Act
        var results = new List<CO2Dto>();
        foreach (var co2 in co2s)
        {
            var result = await dao.CreateAsync(co2);
            results.Add(result);
        }

        //Assert
        Assert.IsNotNull(results);
        Assert.AreEqual(3, results.Count);
        Assert.AreEqual(co2s[0].Date, results[0].Date);
        Assert.AreEqual(co2s[0].Value, results[0].Value);
        Assert.AreEqual(co2s[1].Date, results[1].Date);
        Assert.AreEqual(co2s[1].Value, results[1].Value);
        Assert.AreEqual(co2s[2].Date, results[2].Date);
        Assert.AreEqual(co2s[2].Value, results[2].Value);
    }

    //B - Boundary
    [TestMethod]
    public async Task SaveCO2_MinimumBoundary_Test()
    {
        var co2 = new CO2
        {
            Date = new DateTime(2023, 04, 23),
            Value = 0
        };

        var savedCO2 = await dao.CreateAsync(co2);

        Assert.IsNotNull(savedCO2);
        Assert.AreEqual(co2.Value, savedCO2.Value);
        Assert.AreEqual(co2.Date, savedCO2.Date);
    }

    [TestMethod]
    public async Task SaveCO2_MaximumBoundary_Test()
    {
        var co2 = new CO2
        {
            Date = new DateTime(2023, 04, 23),
            Value = 4095
        };

        var savedCO2 = await dao.CreateAsync(co2);

        Assert.IsNotNull(savedCO2);
        Assert.AreEqual(co2.Value, savedCO2.Value);
        Assert.AreEqual(co2.Date, savedCO2.Date);
    }


    //GetAsync() test
    //Z - Zero
    [TestMethod]
    public async Task GetAsync_Empty_Test()
    {
        // Arrange
        var search = new SearchMeasurementDto(false, null, null);

        // Act
        var result = await dao.GetAsync(search);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    //O - One
    [TestMethod]
    public async Task GetAsync_One_Test()
    {
        //Arrange
        var parameters = new SearchMeasurementDto(true);
        var co2 = new CO2
        {
            Date = new DateTime(2023, 4, 10),
            Value = 300
        };
        await DbContext.CO2s.AddAsync(co2);
        await DbContext.SaveChangesAsync();

        //Assert
        var co2Dtos = await dao.GetAsync(parameters);

        //Act
        Assert.IsNotNull(co2Dtos);
        Assert.AreEqual(1, co2Dtos.Count());
        Console.WriteLine(co2.CO2Id);
        Console.WriteLine(co2Dtos.First().CO2Id);
        Assert.AreEqual(1, co2Dtos.First().CO2Id);
        Assert.AreEqual(co2.Value, co2Dtos.First().Value);
        Assert.AreEqual(co2.Date, co2Dtos.First().Date);
    }


    [TestMethod]
    public async Task GetAsync_Many_WhenNoFiltersApplied_Test()
    {
        // Arrange
        var co21 = new CO2 { Date = new DateTime(2023, 4, 10, 10, 10, 0), Value = 500 };
        var co22 = new CO2 { Date = new DateTime(2023, 4, 11, 10, 10, 0), Value = 600 };
        await DbContext.CO2s.AddAsync(co21);
        await DbContext.CO2s.AddAsync(co22);
        await DbContext.SaveChangesAsync();
        var search = new SearchMeasurementDto(false, null, null);

        // Act
        var result = await dao.GetAsync(search);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(c => c.CO2Id == co21.CO2Id && c.Value == co21.Value && c.Date == co21.Date));
        Assert.IsTrue(result.Any(c => c.CO2Id == co22.CO2Id && c.Value == co22.Value && c.Date == co22.Date));
    }

    [TestMethod]
    public async Task GetAsync_ReturnsDataFilteredByStartDate_Test()
    {
        // Arrange
        var co21 = new CO2 { Date = new DateTime(2023, 4, 9), Value = 500 };
        var co22 = new CO2 { Date = new DateTime(2023, 4, 11), Value = 600 };
        await DbContext.CO2s.AddAsync(co21);
        await DbContext.CO2s.AddAsync(co22);
        await DbContext.SaveChangesAsync();
        var search = new SearchMeasurementDto(false, new DateTime(2023, 4, 11), null);

        // Act
        var result = await dao.GetAsync(search);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Any(c => c.CO2Id == co22.CO2Id && c.Value == co22.Value && c.Date == co22.Date));
    }

    [TestMethod]
    public async Task GetAsync_ReturnsDataFilteredByEndDate_Test()
    {
        // Arrange
        var co21 = new CO2 { Date = new DateTime(2023, 4, 10), Value = 500 };
        var co22 = new CO2 { Date = new DateTime(2023, 5, 11), Value = 600 };
        await DbContext.CO2s.AddAsync(co21);
        await DbContext.CO2s.AddAsync(co22);
        await DbContext.SaveChangesAsync();
        var search = new SearchMeasurementDto(false, null, new DateTime(2023, 5, 1));

        // Act
        var result = await dao.GetAsync(search);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.IsTrue(result.Any(c => c.CO2Id == co21.CO2Id && c.Value == co21.Value && c.Date == co21.Date));
    }

}
