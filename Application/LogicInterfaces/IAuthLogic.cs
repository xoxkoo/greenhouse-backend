using Domain.Entities;

namespace Application.Services;

public interface IAuthLogic
{
    Task<User> ValidateUser(string email, string password);
}