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
        var emailEntity = new Email() { EmailAddress = validEmail };
        _mockEmailDao.Setup(dao => dao.CreateAsync(emailEntity)).ReturnsAsync(emailDto);

        // Act
        var result = await _emailLogic.CreateAsync(emailDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(validEmail, result.EmailAdress);
        _mockEmailDao.Verify(dao => dao.CreateAsync(emailEntity), Times.Once);
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
    
    [TestMethod]
    public async Task CreateAsync_InvalidEmail_ThrowsArgumentException_Test()
    {
        // Arrange
        string invalidEmail = "test@example.com";
        var emailDto = new EmailDto() { EmailAdress = invalidEmail };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await _emailLogic.CreateAsync(emailDto));
    }
    
    //GetAsync()
    [TestMethod]
    public async Task GetAsync_ReturnsEmailDto()
    {
        // Arrange
        string validEmail = "test@gmail.com";
        var emailEntity = new Email() { EmailAddress = validEmail };
        var emailDto = new EmailDto() { EmailAdress = validEmail };
        _mockEmailDao.Setup(dao => dao.GetAsync()).ReturnsAsync(emailDto);

        // Act
        var result = await _emailLogic.GetAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(validEmail, result.EmailAdress);
        _mockEmailDao.Verify(dao => dao.GetAsync(), Times.Once);
    }
}