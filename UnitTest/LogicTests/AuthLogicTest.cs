using Application.DaoInterfaces;
using Application.Services;
using Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Testing.LogicTests;

[TestClass]
public class AuthLogicTest
{
    private Mock<IUserDao> userDaoMock;
    private IAuthLogic _authLogic;

    [TestInitialize]
    public void TestInitialize()
    {
        userDaoMock = new Mock<IUserDao>();
        _authLogic = new AuthLogic(userDaoMock.Object);
    }
    [TestMethod]
    public async Task ValidateUser_ValidCredentials()
    {
        // Arrange
        string email = "test@example.com";
        string password = "password123";
        
        User existingUser = new User
        {
            Email = email,
            Password = password
        };

        userDaoMock.Setup(u => u.GetByEmailAsync(email)).ReturnsAsync(existingUser);

        // Act
        User result = await _authLogic.ValidateUser(email, password);

        // Assert
        Assert.AreEqual(existingUser, result);
    }

    [TestMethod]
    public async Task ValidateUser_UserNotFound_ThrowsException()
    {
        // Arrange
        string email = "nonexistent@example.com";
        string password = "password123";
        User user = null;
        userDaoMock.Setup(u => u.GetByEmailAsync(email)).ReturnsAsync(user);


        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _authLogic.ValidateUser(email, password));

    }

    [TestMethod]
    public async Task ValidateUser_PasswordMismatch_ThrowsException()
    {
        // Arrange
        string email = "test@example.com";
        string password = "aaaaaa";

        User existingUser = new User
        {
            Email = email,
            Password = "tsDhWlceLpAKNa8JYWus0y7/OzM++1S1FvSSS9OvqsLHgkoW"
        };

        userDaoMock.Setup(u => u.GetByEmailAsync(email)).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => _authLogic.ValidateUser(email, password));

    }
}
