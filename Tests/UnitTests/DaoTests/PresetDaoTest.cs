using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Utils;

namespace Tests.UnitTests.DaoTests;

[TestClass]
public class PresetDaoTest :  DbTestBase
{

    private IPresetDao _presetDao;

    [TestInitialize]
    public void TestInitialize()
    {
        _presetDao = new PresetEfcDao(DbContext);
    }

    //GetAsync()
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
        Assert.AreEqual(10, result.First().Thresholds.First().Max);
        Assert.AreEqual(0, result.First().Thresholds.First().Min);
        Assert.AreEqual("co2", result.First().Thresholds.Last().Type);
        Assert.AreEqual(1250, result.First().Thresholds.Last().Max);
        Assert.AreEqual(1200, result.First().Thresholds.Last().Min);
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
        Assert.AreEqual(10, result.First().Thresholds.First().Max);
        Assert.AreEqual(0, result.First().Thresholds.First().Min);
        Assert.AreEqual("co2", result.First().Thresholds.Last().Type);
        Assert.AreEqual(1250, result.First().Thresholds.Last().Max);
        Assert.AreEqual(1200, result.First().Thresholds.Last().Min);
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
        Assert.AreEqual(13, result.First().Thresholds.First().Max);
        Assert.AreEqual(0, result.First().Thresholds.First().Min);
        Assert.AreEqual("co2", result.First().Thresholds.Last().Type);
        Assert.AreEqual(1250, result.First().Thresholds.Last().Max);
        Assert.AreEqual(1230, result.First().Thresholds.Last().Min);
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
        Assert.IsInstanceOfType(result, typeof(List<PresetDto>));
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(1, result.First().Id);
        Assert.AreEqual("Tomato", result.First().Name);
        Assert.AreEqual(true, result.First().IsCurrent);
        Assert.AreEqual(3, result.First().Thresholds.Count());
        Assert.AreEqual("temperature", result.First().Thresholds.First().Type);
        Assert.AreEqual(10, result.First().Thresholds.First().Max);
        Assert.AreEqual(0, result.First().Thresholds.First().Min);
        Assert.AreEqual("co2", result.First().Thresholds.Last().Type);
        Assert.AreEqual(1250, result.First().Thresholds.Last().Max);
        Assert.AreEqual(1200, result.First().Thresholds.Last().Min);
    }

    //B - Boundary
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

    //CreateAsync()
    [TestMethod]
    public async Task CreateAsync_CreatesPresetAndReturnsPresetDto()
    {
        // Arrange
        var preset = new Preset
        {
            Id = 1,
            Name = "Test Preset",
            Thresholds = new List<Threshold>
            {
                new Threshold { Type = "Type1", MinValue = 0, MaxValue = 10 },
                new Threshold { Type = "Type2", MinValue = 20, MaxValue = 30 }
            }
        };

        // Act
        var result = await _presetDao.CreateAsync(preset);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(preset.Id, result.Id);
        Assert.AreEqual(preset.Name, result.Name);
        Assert.AreEqual(preset.Thresholds.Count(), result.Thresholds.Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task CreateAsync_ThrowsExceptionWhenPresetIsNull()
    {
        // Arrange
        Preset preset = null;

        // Act
        await _presetDao.CreateAsync(preset);

        // The test should throw ArgumentNullException
    }


    //ApplyAsync(int id)
    //Z - Zero
    [TestMethod]
    public async Task ApplyAsync_NoValueInSetUp()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => _presetDao.ApplyAsync(1));
    }

    //O - One
    [TestMethod]
    public async Task ApplyAsync_SetsPresetAsCurrent_OneInSetUp()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            }
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        int presetId = 1;

        // Act
        await _presetDao.ApplyAsync(presetId);

        // Assert
        Preset preset1 = DbContext.Presets.FirstOrDefault(p => p.Id == presetId);
        Assert.IsNotNull(preset1);
        Assert.IsTrue(preset1.IsCurrent);
    }

    //M - Many
    [TestMethod]
    public async Task ApplyAsync_SetsPresetAsCurrent_ManyInSetUp()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = false,
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

        int presetId = 2;

        // Act
        await _presetDao.ApplyAsync(presetId);

        // Assert
        Preset? preset2 = await DbContext.Presets.FirstOrDefaultAsync(p => p.Id == presetId);
        Assert.IsNotNull(preset2);
        Assert.IsTrue(preset2.IsCurrent);

        // Other presets should remain unchanged
        Preset? preset1 = await DbContext.Presets.FirstOrDefaultAsync(p => p.Id == 1);
        Assert.IsNotNull(preset1);
        Assert.IsFalse(preset1.IsCurrent);
    }

    [TestMethod]
    public async Task ApplyAsync_SetsPresetAsCurrent_WhenExistingCurrent()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Preset 1",
                IsCurrent = false
            },
            new Preset
            {
                Id = 2,
                Name = "Preset 2",
                IsCurrent = false
            },
            new Preset
            {
                Id = 3,
                Name = "Preset 3",
                IsCurrent = true
            }
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        var presetId = 2;
        var presetDao = new PresetEfcDao(DbContext);

        // Act
        await presetDao.ApplyAsync(presetId);

        // Assert
        var updatedPreset = await DbContext.Presets.FindAsync(presetId);
        Assert.IsNotNull(updatedPreset);
        Assert.IsTrue(updatedPreset.IsCurrent);

        var nonCurrentPresets = DbContext.Presets.Where(p => p.Id != presetId);
        foreach (var preset in nonCurrentPresets)
        {
            Assert.IsFalse(preset.IsCurrent);
        }
    }

    //E - Exception
    [TestMethod]
    public async Task ApplyAsync_ThrowsExceptionForInvalidPresetId()
    {
        // Arrange
        var presets = new List<Preset>
        {
            new Preset
            {
                Id = 1,
                Name = "Tomato",
                IsCurrent = false,
                Thresholds = new List<Threshold>
                {
                    new Threshold { Id = 1, Type = "temperature", MaxValue = 10, MinValue = 0 },
                    new Threshold { Id = 2, Type = "humidity", MaxValue = 60, MinValue = 50 },
                    new Threshold { Id = 3, Type = "co2", MaxValue = 1250, MinValue = 1200 }
                }
            },
        };
        DbContext.Presets.AddRange(presets);
        await DbContext.SaveChangesAsync();

        int presetId = 2; // Invalid preset ID

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => _presetDao.ApplyAsync(presetId));
    }


    [TestMethod]
    public async Task DeleteAsync_testGetById()
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
        // Act
        await _presetDao.CreateAsync(preset);
        await DbContext.SaveChangesAsync();
        Preset preset3 = await _presetDao.GetByIdAsync(1);
        Assert.AreEqual(preset3.Name, preset.Name);
        
    }
    [TestMethod]
    public async Task DeleteAsync_TestDelete()
    {
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
            // Act
            await _presetDao.CreateAsync(preset);
            await DbContext.SaveChangesAsync();
            Assert.AreEqual(1, DbContext.Presets.Local.Count);
            await _presetDao.DeleteAsync(preset);
            Assert.AreEqual(0, DbContext.Presets.Local.Count);
    }
    [TestMethod]
    public async Task DeleteAsync_TestDeleteCorrectPreset()
    {
        var preset1 = new Preset
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
        var preset2 = new Preset
        {
            Id = 2,
            Name = "Tomato",
            IsCurrent = false,
            Thresholds = new List<Threshold>
            {
                new Threshold { Id = 4, Type = "temperature", MaxValue = 10, MinValue = 0 },
                new Threshold { Id = 5, Type = "humidity", MaxValue = 50, MinValue = 60 },
                new Threshold { Id = 6, Type = "co2", MaxValue = 1200, MinValue = 1250 }
            }
        };
        // Act
        await _presetDao.CreateAsync(preset1);
        await DbContext.SaveChangesAsync();
        await _presetDao.CreateAsync(preset2);
        await DbContext.SaveChangesAsync();
        await _presetDao.DeleteAsync(preset1);
        await DbContext.SaveChangesAsync();
        Assert.AreEqual(2, preset2.Id);
    }

}
