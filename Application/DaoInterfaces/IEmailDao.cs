using Domain.DTOs;
using Domain.Entities;

namespace Application.DaoInterfaces;

public interface IEmailDao
{
    Task<EmailDto> CreateAsync(NotificationEmail notificationEmail);
    Task<EmailDto> GetAsync();
}