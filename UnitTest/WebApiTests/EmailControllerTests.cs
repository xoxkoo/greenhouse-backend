using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using WebAPI.Controllers;

namespace Testing.WebApiTests;

[TestClass]
public class EmailControllerTests
{
    private Mock<IEmailLogic> _mockEmailLogic;
    private EmailController _emailController;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockEmailLogic = new Mock<IEmailLogic>();
        _emailController = new EmailController(_mockEmailLogic.Object);
    }
    
    //Z - Zero
    [TestMethod]
    public async Task CreateAsync_NullInput_ReturnsBadRequest()
    {
        // Arrange
        EmailDto emailDto = null;

        // Act
        var result = await _emailController.CreateAsync(emailDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }
    
    //O - One
    [TestMethod]
    public async Task CreateAsync_ValidInput_ReturnsOk()
    {
        // Arrange
        var emailDto = new EmailDto { Email = "example@gmail.com" };
        var createdDto = new EmailDto { Email = "example@gmail.com" };
        _mockEmailLogic.Setup(x => x.CreateAsync(It.IsAny<EmailDto>()))
            .ReturnsAsync(createdDto);

        // Act
        var result = await _emailController.CreateAsync(emailDto);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
        var okResult = (CreatedResult)result.Result;
        Assert.AreEqual(createdDto, okResult.Value);
    }
    
    //E - Exception
    [TestMethod]
    public async Task CreateAsync_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        var emailDto = new EmailDto { Email = "example@example.com" };
        _mockEmailLogic.Setup(x => x.CreateAsync(It.IsAny<EmailDto>()))
            .ThrowsAsync(new ArgumentException("Email address must end with @gmail.com"));
        
        // Act
        var result = await _emailController.CreateAsync(emailDto);
        
        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        ObjectResult statusCodeResult = (ObjectResult)result.Result;
        Assert.AreEqual(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }
    
    [TestMethod]
    public async Task GetAsync_InternalError_ReturnsInternalServerError()
    {
        // Arrange
        _mockEmailLogic.Setup(x => x.GetAsync())
            .ThrowsAsync(new Exception("Error"));

        // Act
        var result = await _emailController.GetAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(ObjectResult));
        var objectResult = (ObjectResult)result.Result;
        Assert.AreEqual(500, objectResult.StatusCode);
    }
    
    
    //GetAsync()
    [TestMethod]
    public async Task GetAsync_ReturnsOKResult()
    {
        //Arrange
        var email1 = new EmailDto { Email = "example@gmail.com" };
        _mockEmailLogic.Setup(x => x.GetAsync())
            .ReturnsAsync(email1);
        
        //Act
        var result = await _emailController.GetAsync();
        
        //Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result.Result;
        Assert.IsInstanceOfType(okResult.Value, typeof(EmailDto));
        var emailResult = (EmailDto)okResult.Value;
        Assert.AreEqual(email1.Email, emailResult.Email);
    }
}