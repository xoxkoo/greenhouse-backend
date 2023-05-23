using Domain.Entities;

namespace Application.Services;

public interface IAuthService
{
    Task<User> ValidateUser(string email, string password);
}