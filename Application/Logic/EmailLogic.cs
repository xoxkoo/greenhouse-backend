using System.Net;
using System.Net.Mail;
using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;

namespace Application.Logic;

public class EmailLogic : IEmailLogic
{
    private readonly IEmailDao _emailDao;

    public EmailLogic(IEmailDao emailDao)
    {
        _emailDao = emailDao;
    }

    public async Task<EmailDto> CreateAsync(EmailDto dto)
    {
        if (!dto.EmailAdress.EndsWith("@gmail.com"))
        {
            throw new ArgumentException("Email address must end with @gmail.com");
        }

        var entity = new Email()
        {
            EmailAddress = dto.EmailAdress
        };

        return await _emailDao.CreateAsync(entity);
    }

    public async Task<EmailDto> GetAsync()
    {
        return await _emailDao.GetAsync();
    }
    
    private SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
    {
        Port = 587,
        Credentials = new NetworkCredential("greenhousex4@gmail.com", "kdrwjmjydpeojmrs"),
        EnableSsl = true,
    };


    public void sendMail(Email mail)
    {
        MailMessage _message = new MailMessage
        {
            From = new MailAddress("greenhousex4@gmail.com"),
            Subject = mail.Title,
            Body = "<h1>" + mail.Body + "<h1>",
            IsBodyHtml = true
        };
        
        _message.To.Add(mail.EmailAddress);
        smtpClient.Send(_message);
    }

}