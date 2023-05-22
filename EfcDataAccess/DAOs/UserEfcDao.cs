using Application.DaoInterfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess.DAOs;

public class UserEfcDao : IUserDao
{
    private readonly Context _context;

    public UserEfcDao(Context context)
    {
        _context = context;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.Where(u => u.Email.Equals(email)).FirstOrDefaultAsync();
    }
}