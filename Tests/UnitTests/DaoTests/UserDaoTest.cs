using Application.DaoInterfaces;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Utils;

namespace Tests.UnitTests.DaoTests;

[TestClass]
public class UserDaoTest : DbTestBase
{
    private IUserDao _userDao;


    [TestInitialize]
    public void TestInitialize()
    {
        _userDao = new UserEfcDao(DbContext);
    }
    
    [TestMethod]
    public async Task GetByEmailAsync_UserExists_ReturnsUser()
    {
        // Arrange
        string email = "test@example.com";
        string password = "password";
        User expectedUser = new User
        {
            Email = email,
            Password = password
        };
        
        DbContext.Users.Add(expectedUser);
        DbContext.SaveChanges();

        // Act
        User result = await _userDao.GetByEmailAsync(email);

        // Assert
        Assert.AreEqual(expectedUser, result);
        }
    

    [TestMethod]
    public async Task GetByEmailAsync_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        string email = "nonexistent@example.com";

        // Act
        User result = await _userDao.GetByEmailAsync(email);

        // Assert
        Assert.IsNull(result);
        }
}
