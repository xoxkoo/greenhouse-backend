using Domain.DTOs;
using Domain.Entities;

namespace Application.LogicInterfaces;

public interface IEmailLogic
{
    Task<EmailDto> CreateAsync(EmailDto dto);
    Task<EmailDto> GetAsync();
    void sendMail(Email mail);
}