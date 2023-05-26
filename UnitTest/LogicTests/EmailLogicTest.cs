using System.Net;
using System.Net.Mail;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Testing.WebApiTests;

[TestClass]
public class EmailLogicTest
{
    private Mock<IEmailDao> _mockEmailDao;
    private Mock<IPresetDao> _mockPresetDao;
    private IEmailLogic _emailLogic;


    [TestInitialize]
    public void TestInitialize()
    {
        _mockEmailDao = new Mock<IEmailDao>();
        _mockPresetDao = new Mock<IPresetDao>();
        _emailLogic = new EmailLogic(_mockEmailDao.Object, _mockPresetDao.Object);
    }

    //CreateAsync(EmailDto dto) tests
    //Z - Zero
    [TestMethod]
    public async Task CreateAsync_EmailDtoIsNull_ThrowsArgumentNullException_Test()
    {
        // Arrange
        EmailDto nullDto = null;

        // Act and Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _emailLogic.CreateAsync(nullDto));
    }

    //O - One
    [TestMethod]
    public async Task CreateAsync_ValidEmail_Test()
    {
        // Arrange
        string validEmail = "test@gmail.com";
        var emailDto = new EmailDto() { EmailAdress = validEmail };
        _mockEmailDao.Setup(dao => dao.CreateAsync(It.IsAny<NotificationEmail>())).ReturnsAsync(emailDto);
        // Act
        var result = await _emailLogic.CreateAsync(emailDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(validEmail, result.EmailAdress);
    }

    //E - Exception
    [TestMethod]
    public async Task CreateAsync_EmailAddressIsEmpty_ThrowsArgumentException()
    {
        //Arrange
        var dto = new EmailDto
        {
            EmailAdress = ""
        };

        //Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _emailLogic.CreateAsync(dto));
    }
    

    //GetAsync()
    [TestMethod]
    public async Task GetAsync_ReturnsEmailDto()
    {
        // Arrange
        string validEmail = "test@gmail.com";
        var emailEntity = new NotificationEmail() { Email = validEmail };
        var emailDto = new EmailDto() { EmailAdress = validEmail };
        _mockEmailDao.Setup(dao => dao.GetAsync()).ReturnsAsync(emailDto);

        // Act
        var result = await _emailLogic.GetAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(validEmail, result.EmailAdress);
        _mockEmailDao.Verify(dao => dao.GetAsync(), Times.Once);
    }


    [TestMethod]
    public async Task CheckIfInRange_ShouldSendEmail()
    {
        // Arrange
        _mockEmailDao.Setup(e => e.GetAsync()).ReturnsAsync(new EmailDto { EmailAdress = "greenhousesep4@gmail.com" });

        var temperature = 30;
        var humidity = 80;
        var co2 = 2000;

        var thresholds = new List<ThresholdDto>
        {
            new ThresholdDto() { Type = "temperature", Min = 10, Max = 25 },
            new ThresholdDto() { Type = "humidity", Min = 30, Max = 60 },
            new ThresholdDto() { Type = "co2", Min = 400, Max = 1000 },
        };

        var presetDto = new PresetDto { Thresholds = thresholds };
        _mockPresetDao.Setup(d => d.GetAsync(It.IsAny<SearchPresetParametersDto>()))
            .ReturnsAsync(new List<PresetDto> { presetDto });

        // Act
        await _emailLogic.CheckIfInRange(temperature, humidity, co2);
    }
}
