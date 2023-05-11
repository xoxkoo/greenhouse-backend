using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing.DaoTests;

[TestClass]
public class PresetDaoTest :  DbTestBase
{

    private IPresetDao _presetDao;

    [TestInitialize]
    public void TestInitialize()
    {
        _presetDao = new PresetEfcDao(DbContext);
    }

    //Z - Zero
    [TestMethod]
    public async Task GetAsync_ReturnsZeroPreset()
    {
        //Act
        var result = await _presetDao.GetAsync(new SearchPresetParametersDto());

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }
    
    //O - One
    [TestMethod]
    public async Task GetAsync_ReturnsOnePreset()
    {
        //Arrange
        var preset = new Preset
        {
            Id = 1,
            Name = "Tomato",
            IsCurrent = true,
            Thresholds = new List<Threshold>
            {
                new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                new Threshold { Id = 2, Type = "humidity", MaxValue = 50, MinValue = 60 },
                new Threshold { Id = 3, Type = "co2", MaxValue = 1200, MinValue = 1250 }
            }
        };
        await DbContext.AddAsync(preset);
        await DbContext.SaveChangesAsync();
        
        //Act
        var result = await _presetDao.GetAsync(new SearchPresetParametersDto());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());

        var preset1 = result.First(p => p.Id == 1);
        Assert.AreEqual("Tomato", preset1.Name);
        Assert.IsTrue(preset1.IsCurrent);
        Assert.IsNotNull(preset1.Thresholds);
        Assert.AreEqual(3, preset1.Thresholds.Count());
    }
    
    //M - Many
    [TestMethod]
    public async Task GetAsync_ReturnsAllPresets()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            },
            new Preset
            {
                Id = 2,
                Name = "Sunny Day",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 4, Type = "temperature", MaxValue = 13, MinValue = 0 },
                    new Threshold { Id = 5, Type = "humidity", MaxValue = 55, MinValue = 60 },
                    new Threshold { Id = 6, Type = "co2", MaxValue = 1230, MinValue = 1250 }
                }
            },
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _presetDao.GetAsync(new SearchPresetParametersDto());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(presets.Count, result.Count());

        var preset1 = result.First(p => p.Id == 1);
        Assert.AreEqual("Tomato", preset1.Name);
        Assert.IsTrue(preset1.IsCurrent);
        Assert.IsNotNull(preset1.Thresholds);
        Assert.AreEqual(3, preset1.Thresholds.Count());

        var preset2 = result.First(p => p.Id == 2);
        Assert.AreEqual("Sunny Day", preset2.Name);
        Assert.IsFalse(preset2.IsCurrent);
        Assert.IsNotNull(preset2.Thresholds);
        Assert.AreEqual(3, preset2.Thresholds.Count());
    }


    [TestMethod]
    public async Task GetAsync_GetCurrentPreset()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            },
            new Preset
            {
                Id = 2,
                Name = "Sunny Day",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 4, Type = "temperature", MaxValue = 13, MinValue = 0 },
                    new Threshold { Id = 5, Type = "humidity", MaxValue = 55, MinValue = 60 },
                    new Threshold { Id = 6, Type = "co2", MaxValue = 1230, MinValue = 1250 }
                }
            },
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        var searchParams = new SearchPresetParametersDto(null, true);

        // Act
        var result = await _presetDao.GetAsync(searchParams);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<PresetDto>));
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(1, result.First().Id);
        Assert.AreEqual("Tomato", result.First().Name);
        Assert.AreEqual(true, result.First().IsCurrent);
        Assert.AreEqual(3, result.First().Thresholds.Count());
        Assert.AreEqual("temperature", result.First().Thresholds.First().Type);
        Assert.AreEqual(10, result.First().Thresholds.First().MaxValue);
        Assert.AreEqual(0, result.First().Thresholds.First().MinValue);
        Assert.AreEqual("co2", result.First().Thresholds.Last().Type);
        Assert.AreEqual(1250, result.First().Thresholds.Last().MaxValue);
        Assert.AreEqual(1200, result.First().Thresholds.Last().MinValue);
    }
    
    [TestMethod]
    public async Task GetAsync_GetById()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            },
            new Preset
            {
                Id = 2,
                Name = "Sunny Day",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 4, Type = "temperature", MaxValue = 13, MinValue = 0 },
                    new Threshold { Id = 5, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 6, Type = "co2", MaxValue = 1250, MinValue = 1230 }
                }
            },
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        var searchParams = new SearchPresetParametersDto(1, null);

        // Act
        var result = await _presetDao.GetAsync(searchParams);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<PresetDto>));
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(1, result.First().Id);
        Assert.AreEqual("Tomato", result.First().Name);
        Assert.AreEqual(true, result.First().IsCurrent);
        Assert.AreEqual(3, result.First().Thresholds.Count());
        Assert.AreEqual("temperature", result.First().Thresholds.First().Type);
        Assert.AreEqual(10, result.First().Thresholds.First().MaxValue);
        Assert.AreEqual(0, result.First().Thresholds.First().MinValue);
        Assert.AreEqual("co2", result.First().Thresholds.Last().Type);
        Assert.AreEqual(1250, result.First().Thresholds.Last().MaxValue);
        Assert.AreEqual(1200, result.First().Thresholds.Last().MinValue);
    }
    
    [TestMethod]
    public async Task GetAsync_AppliedParametersIdAndIsCurrent()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            },
            new Preset
            {
                Id = 2,
                Name = "Sunny Day",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 4, Type = "temperature", MaxValue = 13, MinValue = 0 },
                    new Threshold { Id = 5, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 6, Type = "co2", MaxValue = 1250, MinValue = 1230 }
                }
            },
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        var searchParams = new SearchPresetParametersDto(2, false);

        // Act
        var result = await _presetDao.GetAsync(searchParams);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<PresetDto>));
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(2, result.First().Id);
        Assert.AreEqual("Sunny Day", result.First().Name);
        Assert.AreEqual(false, result.First().IsCurrent);
        Assert.AreEqual(3, result.First().Thresholds.Count());
        Assert.AreEqual("temperature", result.First().Thresholds.First().Type);
        Assert.AreEqual(13, result.First().Thresholds.First().MaxValue);
        Assert.AreEqual(0, result.First().Thresholds.First().MinValue);
        Assert.AreEqual("co2", result.First().Thresholds.Last().Type);
        Assert.AreEqual(1250, result.First().Thresholds.Last().MaxValue);
        Assert.AreEqual(1230, result.First().Thresholds.Last().MinValue);
    }
    
    [TestMethod]
    public async Task GetAsync_AppliedParametersIdAndIsCurrent_ExpectedEmptyList()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = true,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            },
            new Preset
            {
                Id = 2,
                Name = "Sunny Day",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 4, Type = "temperature", MaxValue = 13, MinValue = 0 },
                    new Threshold { Id = 5, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 6, Type = "co2", MaxValue = 1250, MinValue = 1230 }
                }
            },
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        var searchParams = new SearchPresetParametersDto(1, false);

        // Act
        var result = await _presetDao.GetAsync(searchParams);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<PresetDto>));
        Assert.AreEqual(0, result.Count());
    }
}