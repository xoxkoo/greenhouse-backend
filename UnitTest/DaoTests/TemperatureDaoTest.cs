using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing.DaoTests;

[TestClass]
public class TemperatureDaoTest : DbTestBase
{
    private ITemperatureDao dao;

    [TestInitialize]
    public void TestInitialize()
    {
        dao = new TemperatureEfcDao(DbContext);
    }

    //CreateAsync() test
    //Z - Zero
    [TestMethod]
    public async Task CreateTemperature_NullException_Test()
    {
        //Arrange
        Temperature temperature = null;

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dao.CreateAsync(temperature));
    }

    //O - One
    [TestMethod]
    public async Task CreateTemperatureTest()
    {
        var temperature = new Temperature
        {
            Date = new DateTime(2023, 4, 10),
            Value = 10
        };

        var createdTemperature = await dao.CreateAsync(temperature);

        Assert.IsNotNull(createdTemperature);
        Assert.AreEqual(1, createdTemperature.TemperatureId);
        Assert.AreEqual(temperature.Value, createdTemperature.Value);
        Assert.AreEqual(((DateTimeOffset)temperature.Date).ToUnixTimeSeconds(), createdTemperature.Date);
    }

    //M - Many
    [TestMethod]
    public async Task CreateTemperature_Many_Test()
    {
        //Arrange
        var temperatures = new List<Temperature>
        {
            new Temperature
            {
                Date = new DateTime(2023, 04, 22),
                Value = 10
            },
            new Temperature
            {
                Date = new DateTime(2023, 04, 23),
                Value = 15
            },
            new Temperature
            {
                Date = new DateTime(2023, 04, 21),
                Value = 17
            }
        };

        //Act
        var results = new List<TemperatureDto>();
        foreach (var temp in temperatures)
        {
            var result = await dao.CreateAsync(temp);
            results.Add(result);
        }

        //Assert
        Assert.IsNotNull(results);
        Assert.AreEqual(3, results.Count);
        Assert.AreEqual(((DateTimeOffset)temperatures[0].Date).ToUnixTimeSeconds(), results[0].Date);
        Assert.AreEqual(temperatures[0].Value, results[0].Value);
        Assert.AreEqual(((DateTimeOffset)temperatures[1].Date).ToUnixTimeSeconds(), results[1].Date);
        Assert.AreEqual(temperatures[1].Value, results[1].Value);
        Assert.AreEqual(((DateTimeOffset)temperatures[2].Date).ToUnixTimeSeconds(), results[2].Date);
        Assert.AreEqual(temperatures[2].Value, results[2].Value);
    }

    //B - Boundary
    [TestMethod]
    public async Task CreateTemperature_MinimumBoundary_Test()
    {
        var temperature = new Temperature
        {
            Date = new DateTime(2023, 04, 23),
            Value = -50
        };

        var createdTemperature = await dao.CreateAsync(temperature);

        Assert.IsNotNull(createdTemperature);
        Assert.AreEqual(temperature.Value, createdTemperature.Value);
        Assert.AreEqual(((DateTimeOffset)temperature.Date).ToUnixTimeSeconds(), createdTemperature.Date);
    }

    [TestMethod]
    public async Task CreateTemperature_MaximumBoundary_Test()
    {
        var temperature = new Temperature
        {
            Date = new DateTime(2023, 04, 23),
            Value = 60
        };

        var createdTemperature = await dao.CreateAsync(temperature);

        Assert.IsNotNull(createdTemperature);
        Assert.AreEqual(temperature.Value, createdTemperature.Value);
        Assert.AreEqual(((DateTimeOffset)temperature.Date).ToUnixTimeSeconds(), createdTemperature.Date);
    }



    //GetAsync() test
    //Z - Zero
    [TestMethod]
    public async Task GetAsync_Zero_Test()
    {
        //Arrange
        var search = new SearchMeasurementDto(false, null, null);

        //Act
        var result = await dao.GetAsync(search);

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
        Assert.IsFalse(result.Any());
    }

    //O - one
    [TestMethod]
    public async Task GetAsync_One_Test()
    {
        //Arrange
        var parameters = new SearchMeasurementDto(true);
        var temperature = new Temperature
        {
            Date = new DateTime(2023, 4, 10),
            Value = 10
        };
        await DbContext.Temperatures.AddAsync(temperature);
        await DbContext.SaveChangesAsync();

        //Assert
        var temperatureFromDb = await dao.GetAsync(parameters);

        //Act
        Assert.IsNotNull(temperatureFromDb);
        Assert.AreEqual(1, temperatureFromDb.First().TemperatureId);
        Assert.AreEqual(temperature.Value, temperatureFromDb.First().Value);
        Assert.AreEqual(((DateTimeOffset)temperature.Date).ToUnixTimeSeconds(), temperatureFromDb.First().Date);
    }


    //M - Many
    [TestMethod]
    public async Task GetAsync_ManyInDb_Test()
    {
        //Assert
        var temp1 = new Temperature { Date = new DateTime(2023, 04, 01), Value = 10 };
        var temp2 = new Temperature { Date = new DateTime(2023, 04, 02), Value = 20 };
        await DbContext.Temperatures.AddAsync(temp1);
        await DbContext.Temperatures.AddAsync(temp2);
        await DbContext.SaveChangesAsync();

        var dto = new SearchMeasurementDto (false);

        // Act
        var result = await dao.GetAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(((DateTimeOffset)temp1.Date).ToUnixTimeSeconds(), result.FirstOrDefault()?.Date);
        Assert.AreEqual(temp1.Value, result.FirstOrDefault()?.Value);
        Assert.AreEqual(((DateTimeOffset)temp2.Date).ToUnixTimeSeconds(), result.Last().Date);
        Assert.AreEqual(temp2.Value, result.Last().Value);
    }

    [TestMethod]
    public async Task GetAsync_ManyInDb_FilteredByDateRange_Test()
    {
        //Assert
        var temp1 = new Temperature { Date = new DateTime(2023, 01, 02), Value = 10 };
        var temp2 = new Temperature { Date = new DateTime(2023, 04, 02), Value = 20 };
        await DbContext.Temperatures.AddAsync(temp1);
        await DbContext.Temperatures.AddAsync(temp2);
        await DbContext.SaveChangesAsync();

        var dto = new SearchMeasurementDto (false, new DateTime(2023, 01, 01), new DateTime(2023, 02, 01));

        // Act
        var result = await dao.GetAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(((DateTimeOffset)temp1.Date).ToUnixTimeSeconds(), result.FirstOrDefault()?.Date);
        Assert.AreEqual(temp1.Value, result.FirstOrDefault()?.Value);
    }


    [TestMethod]
    public async Task GetAsync_ManyInDb_FilteredByStartDate_Test()
    {
        //Assert
        var temp1 = new Temperature { Date = new DateTime(2023, 01, 02), Value = 10 };
        var temp2 = new Temperature { Date = new DateTime(2023, 04, 02), Value = 20 };
        await DbContext.Temperatures.AddAsync(temp1);
        await DbContext.Temperatures.AddAsync(temp2);
        await DbContext.SaveChangesAsync();

        var dto = new SearchMeasurementDto (false, new DateTime(2023, 03, 01));

        // Act
        var result = await dao.GetAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(((DateTimeOffset)temp2.Date).ToUnixTimeSeconds(), result.FirstOrDefault()?.Date);
        Assert.AreEqual(temp2.Value, result.FirstOrDefault()?.Value);
    }

    [TestMethod]
    public async Task GetAsync_ManyInDb_FilteredByEndDate_Test()
    {
        //Assert
        var temp1 = new Temperature { Date = new DateTime(2023, 01, 02), Value = 10 };
        var temp2 = new Temperature { Date = new DateTime(2023, 02, 02), Value = 20 };
        var temp3 = new Temperature { Date = new DateTime(2023, 03, 02), Value = 20 };
        await DbContext.Temperatures.AddAsync(temp1);
        await DbContext.Temperatures.AddAsync(temp2);
        await DbContext.SaveChangesAsync();

        var dto = new SearchMeasurementDto (false, null, new DateTime(2023, 02, 03));

        // Act
        var result = await dao.GetAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(((DateTimeOffset)temp1.Date).ToUnixTimeSeconds(), result.FirstOrDefault()?.Date);
        Assert.AreEqual(temp1.Value, result.FirstOrDefault()?.Value);
        Assert.AreEqual(((DateTimeOffset)temp2.Date).ToUnixTimeSeconds(), result.Last().Date);
        Assert.AreEqual(temp2.Value, result.LastOrDefault().Value);
    }




}
