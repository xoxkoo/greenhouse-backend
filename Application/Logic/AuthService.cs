using Application.DaoInterfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application.Services;

public class AuthService : IAuthService
{
    private IUserDao _userDao;

    public AuthService(IUserDao userDao)
    {
        _userDao = userDao;
    }

    public async Task<User> ValidateUser(string email, string password)
    {
        User? existingUser = await _userDao.GetByEmailAsync(email);

        if (existingUser == null)
        {
            throw new Exception("User not found");
        }

        if (!existingUser.Password.Equals(password))
        {
            throw new Exception("Password mismatch");
        }

        return existingUser;
    }
}