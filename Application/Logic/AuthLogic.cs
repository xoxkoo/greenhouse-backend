using System.Security.Cryptography;
using Application.DaoInterfaces;
using Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;

namespace Application.Services;

public class AuthLogic : IAuthLogic
{
    private IUserDao _userDao;

    public AuthLogic(IUserDao userDao)
    {
        _userDao = userDao;
    }

    public async Task<User> ValidateUser(string email, string password)
    {

        User? existingUser = await _userDao.GetByEmailAsync(email);

        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        bool isPasswordValid = VerifyPassword(password, existingUser.Password);

        if (!isPasswordValid)
        {
            throw new ArgumentException("Password mismatch");
        }

        return existingUser;
    }

    // Compare the hashed password with the input password
    private static bool VerifyPassword(string password, string hashedPassword)
    {
	    byte[] hashBytes = Convert.FromBase64String(hashedPassword);

	    byte[] salt = new byte[16];

	    Buffer.BlockCopy(hashBytes, 20, salt, 0, 16);

	    using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
	    {
		    byte[] hash = pbkdf2.GetBytes(20); // 20-byte hash

		    // Compare the computed hash with the stored hash
		    for (int i = 0; i < 20; i++)
		    {
			    if (hashBytes[i] != hash[i])
			    {
				    return false;
			    }
		    }
	    }

	    return true;
    }

    // Generate a salt for password hashing
    private static byte[] GenerateSalt()
    {
	    byte[] salt = new byte[16];
	    using (var rng = new RNGCryptoServiceProvider())
	    {
		    rng.GetBytes(salt);
	    }
	    return salt;
    }

    // Hash the password using PBKDF2 algorithm
    private static string HashPassword(string password)
    {
	    byte[] salt;
	    byte[] hash;

	    using (var pbkdf2 = new Rfc2898DeriveBytes(password, 16, 10000))
	    {
		    salt = pbkdf2.Salt;
		    hash = pbkdf2.GetBytes(20); // 20-byte hash
	    }

	    byte[] hashBytes = new byte[36]; // 20-byte hash + 16-byte salt
	    Buffer.BlockCopy(hash, 0, hashBytes, 0, 20);
	    Buffer.BlockCopy(salt, 0, hashBytes, 20, 16);

	    return Convert.ToBase64String(hashBytes);
    }
}
