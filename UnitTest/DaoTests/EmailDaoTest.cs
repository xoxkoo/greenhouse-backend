using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess.DAOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing.DaoTests;

[TestClass]
public class EmailDaoTest : DbTestBase
{

    private IEmailDao _emailDao;
    
    [TestInitialize]
    public void TestInitialize()
    {
        _emailDao = new EmailEfcDao(DbContext);
    }
    
    //CreateAsync() tests
    //Z - Zero
    [TestMethod]
    public async Task CreateAsync_WithNullData_Test()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _emailDao.CreateAsync(null));
    }
    
    //O - One
    [TestMethod]
    public async Task CreateAsync_WithValidData_Test()
    {
        // Arrange
        var email = new Email() { EmailAddress = "test@gmail.com" };

        // Act
        var result = await _emailDao.CreateAsync(email);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(EmailDto));
        Assert.AreEqual(email.EmailAddress, result.Email);
    }

    //M - Many
    [TestMethod]
    public async Task CreateAsync_WithExistingEmailAddress_Test()
    {
        // Arrange
        var email1 = new Email() { EmailAddress = "test@gmail.com" };
        var email2 = new Email() { EmailAddress = "test2@gmail.com" };
        await _emailDao.CreateAsync(email1);

        // Act
        await _emailDao.CreateAsync(email2);

        // Assert
        var result = DbContext.Mails.AsEnumerable();
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual(email2.EmailAddress, result.FirstOrDefault().EmailAddress);
    }

    
    
    //GetAsync() tests
    //Z - Zero
    [TestMethod]
    public async Task GetAsync_WithNoEmail_Test()
    {
        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(async () => await _emailDao.GetAsync(),
            "No email found.");
    }
    
    //O - One
    [TestMethod]
    public async Task GetAsync_WithExistingEmail_Test()
    {
        // Arrange
        var email = new Email() { EmailAddress = "test@gmail.com" };
        await DbContext.AddAsync(email);
        await DbContext.SaveChangesAsync();
        
        // Act
        var result = await _emailDao.GetAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(EmailDto));
        Assert.AreEqual(email.EmailAddress, result.Email);
    }
}