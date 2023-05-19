using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IUserDao
{
    Task<User> GetByEmailAsync(string email);
}