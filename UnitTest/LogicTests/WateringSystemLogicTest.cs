﻿using Application.DaoInterfaces;
using Application.Logic;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Testing.Utils;

namespace Testing.WebApiTests;
[TestClass]
public class WateringSystemLogicTest : DbTestBase
{
    private Mock<IWateringSystemDao> dao;
    private IWateringSystemLogic logic;
    
    [TestInitialize]
    public void WateringSystemLogicTestInit()
    {
        base.TestInit();
        dao = new Mock<IWateringSystemDao>();
        logic = new WateringSystemLogic(dao.Object);
    }
    [TestMethod]
    public async Task WateringSystemDurationTest()
    {
        dao.Setup(dao => dao.CreateAsync(It.IsAny<ValveState>()))
            .ReturnsAsync(new ValveStateDto() {Toggle = true});
        ValveStateCreationDto dto = new ValveStateCreationDto()
        {
            duration = 0,
            Toggle = true
        };
        var expectedValues = "Duration cannot be 0 or less";
        try
        {
            ValveStateDto created = await logic.CreateAsync(dto);
        }
        catch (Exception e)
        {
            Assert.AreEqual(expectedValues, e.Message);
        }
    }
    //TODO fix
    [TestMethod]
    public async Task WateringSystemDurationNullTest()
    {
        dao.Setup(dao => dao.CreateAsync(It.IsAny<ValveState>()))
            .ReturnsAsync(new ValveStateDto() {});
        ValveStateCreationDto dto = new ValveStateCreationDto()
        {
        };
        var expectedValues = "Duration cannot be 0 or less";
        try
        {
            ValveStateDto created = await logic.CreateAsync(dto);
        }
        catch (Exception e)
        {
            Assert.AreEqual(expectedValues, e.Message);
        }
    }
    [TestMethod]
    public async Task GetReturnsExpectedValue()
    {
        dao.Setup(dao => dao.GetAsync())
            .ReturnsAsync(new ValveStateDto() {Toggle = false});
        ValveStateCreationDto dto = new ValveStateCreationDto()
        {
            duration = 2,
            Toggle = false
        };
        ValveStateDto created = await logic.GetAsync();
        Assert.AreEqual(created.Toggle, dto.Toggle);
    }
    
    [TestMethod]
    public async Task CreateAsync_WhenDurationIsNull_ThrowsException()
    {
        // Arrange
        ValveStateCreationDto dto = null;

        // Act & Assert
        Assert.ThrowsExceptionAsync<Exception>(() => logic.CreateAsync(dto));
    }

    [TestMethod]
    public async Task CreateAsync_WhenToggleIsTrueAndDurationIsLessThanOrEqualToZero_ThrowsException()
    {
        // Arrange
        var dto = new ValveStateCreationDto { duration = 0, Toggle = true };

        // Act & Assert
        Assert.ThrowsExceptionAsync<Exception>(() => logic.CreateAsync(dto));
    }

    [TestMethod]
    public async Task CreateAsync_WhenValidDtoIsPassed_CreatesNewValveState()
    {
        dao.Setup(dao => dao.CreateAsync(It.IsAny<ValveState>()))
            .ReturnsAsync(new ValveStateDto() {Toggle = false});
        // Arrange
        var dto = new ValveStateCreationDto() { duration = 10, Toggle = false };

        // Act
        var result = await logic.CreateAsync(dto);
        Console.WriteLine(result.Toggle);
        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<ValveStateDto>(result);
    }
}