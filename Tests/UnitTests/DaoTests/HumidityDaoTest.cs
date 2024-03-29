﻿using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Utils;

namespace Tests.UnitTests.DaoTests;

[TestClass]
public class HumidityDaoTest : DbTestBase
{
    private IHumidityDao dao;

    [TestInitialize]
    public void TestInitialize()
    {
        dao = new HumidityEfcDao(DbContext);
    }

    //CreateAsync() tests
    //Z - Zero
    [TestMethod]
    public async Task CreateHumidity_NullException_Test()
    {
        //Arrange
        Humidity humidity = null;

        //Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => dao.CreateAsync(humidity));
    }

    //O - One
    [TestMethod]
    public async Task CreateHumidity_OneValue_Test()
    {
        var humidity = new Humidity
        {
            Date = new DateTime(2023, 4, 10),
            Value = 40
        };

        var createdHumidity = await dao.CreateAsync(humidity);

        Assert.IsNotNull(createdHumidity);
        Assert.AreEqual(1, createdHumidity.HumidityId);
        Assert.AreEqual(humidity.Value, createdHumidity.Value);
        Assert.AreEqual(((DateTimeOffset)humidity.Date).ToUnixTimeSeconds(), createdHumidity.Date);
    }

    //M - Many
    [TestMethod]
    public async Task CreateHumidity_Many_Test()
    {
        //Arrange
        var humidities = new List<Humidity>
        {
            new Humidity
            {
                Date = new DateTime(2023, 04, 22),
                Value = 40
            },
            new Humidity
            {
                Date = new DateTime(2023, 04, 23),
                Value = 50
            },
            new Humidity
            {
                Date = new DateTime(2023, 04, 21),
                Value = 60
            }
        };

        //Act
        var results = new List<HumidityDto>();
        foreach (var humidity in humidities)
        {
            var result = await dao.CreateAsync(humidity);
            results.Add(result);
        }

        //Assert
        Assert.IsNotNull(results);
        Assert.AreEqual(3, results.Count);
        Assert.AreEqual(((DateTimeOffset)humidities[0].Date).ToUnixTimeSeconds(), results[0].Date);
        Assert.AreEqual(humidities[0].Value, results[0].Value);
        Assert.AreEqual(((DateTimeOffset)humidities[1].Date).ToUnixTimeSeconds(), results[1].Date);
        Assert.AreEqual(humidities[1].Value, results[1].Value);
        Assert.AreEqual(((DateTimeOffset)humidities[2].Date).ToUnixTimeSeconds(), results[2].Date);
        Assert.AreEqual(humidities[2].Value, results[2].Value);
    }



    //GetAsync() test
    //Z - Zero
    [TestMethod]
    public async Task GetHumidityAsync_ReturnsEmptyList_Test()
    {
        // Arrange
        var search = new SearchMeasurementDto(false, null, null);

        // Act
        var result = await dao.GetAsync(search);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
        Assert.IsFalse(result.Any());
    }


    //O - One
    [TestMethod]
    public async Task GetHumidityAsync_One_Test()
    {
        // Arrange
        var humidity = new Humidity
        {
            Date = new DateTime(2023, 4, 10),
            Value = 10
        };
        await DbContext.Humidities.AddAsync(humidity);
        await DbContext.SaveChangesAsync();

        var parameters = new SearchMeasurementDto(true);

        // Act
        var result = await dao.GetAsync(parameters);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(humidity.HumidityId, result.First().HumidityId);
        Assert.AreEqual(humidity.Value, result.First().Value);
        Assert.AreEqual(((DateTimeOffset)humidity.Date).ToUnixTimeSeconds(), result.First().Date);
    }


    //M - Many
    [TestMethod]
    public async Task GetHumidityAsync_Many_Test()
    {
        // Arrange
        var humidity1 = new Humidity { Date = new DateTime(2023, 04, 01), Value = 10 };
        var humidity2 = new Humidity { Date = new DateTime(2023, 04, 02), Value = 20 };
        await DbContext.Humidities.AddAsync(humidity1);
        await DbContext.Humidities.AddAsync(humidity2);
        await DbContext.SaveChangesAsync();

        var dto = new SearchMeasurementDto(false);

        // Act
        var result = await dao.GetAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(((DateTimeOffset)humidity1.Date).ToUnixTimeSeconds(), result.FirstOrDefault()?.Date);
        Assert.AreEqual(humidity1.Value, result.FirstOrDefault()?.Value);
        Assert.AreEqual(((DateTimeOffset)humidity2.Date).ToUnixTimeSeconds(), result.Last().Date);
        Assert.AreEqual(humidity2.Value, result.Last().Value);
    }

    [TestMethod]
    public async Task GetHumidityAsync_Many_FilteredByStartDate_Test()
    {
        // Arrange
        var humidity1 = new Humidity { Date = new DateTime(2023, 01, 02), Value = 10 };
        var humidity2 = new Humidity { Date = new DateTime(2023, 04, 02), Value = 20 };
        await DbContext.Humidities.AddAsync(humidity1);
        await DbContext.Humidities.AddAsync(humidity2);
        await DbContext.SaveChangesAsync();

        var dto = new SearchMeasurementDto(false, new DateTime(2023, 03, 01));

        // Act
        var result = await dao.GetAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(((DateTimeOffset)humidity2.Date).ToUnixTimeSeconds(), result.FirstOrDefault()?.Date);
        Assert.AreEqual(humidity2.Value, result.FirstOrDefault()?.Value);
    }

    [TestMethod]
    public async Task GetHumidityAsync_Many_FilteredByEndDate_Test()
    {
        // Arrange
        var humidity1 = new Humidity { Date = new DateTime(2023, 01, 02), Value = 10 };
        var humidity2 = new Humidity { Date = new DateTime(2023, 04, 02), Value = 20 };
        await DbContext.Humidities.AddAsync(humidity1);
        await DbContext.Humidities.AddAsync(humidity2);
        await DbContext.SaveChangesAsync();

        var dto = new SearchMeasurementDto(false, null, new DateTime(2023, 03, 01));

        // Act
        var result = await dao.GetAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(((DateTimeOffset)humidity1.Date).ToUnixTimeSeconds(), result.FirstOrDefault()?.Date);
        Assert.AreEqual(humidity1.Value, result.FirstOrDefault()?.Value);
    }
}
