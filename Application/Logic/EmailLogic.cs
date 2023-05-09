using System.Net;
using System.Net.Mail;
using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;

namespace Application.Logic;
public class EmailLogic : IEmailLogic
{
    private static Email temperatureTooLow = new Email()
    {
        Title = "Temperature is too low",
        Body = "The temperature in the greenhouse is too low."
    };
    
    private static Email temperatureTooHigh = new Email()
    {
        Title = "Temperature is too high",
        Body = "The temperature in the greenhouse is too high."
    };
    private static Email humidityTooLow = new Email()
    {
        Title = "Humidity is too low",
        Body = "The humidity in the greenhouse is too low."
    };
    private static Email humidityTooHigh = new Email()
    {
        Title = "Humidity is too high",
        Body = "The humidity in the greenhouse is too high."
    };
    private static Email Co2LvlTooHigh = new Email()
    {
        Title = "CO2 is too high",
        Body = "The CO2 in the greenhouse is too high."
    };    private static Email Co2LvlTooLow = new Email()
    {
        Title = "CO2 is too low",
        Body = "The CO2 in the greenhouse is too low."
    };
    
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
        Credentials = new NetworkCredential("greenhousesep4@gmail.com", "zievqkygqhfrwioe"),
        EnableSsl = true,
    };


    public void sendMail(Email mail)
    {
        MailMessage message = new MailMessage
        {
            From = new MailAddress("greenhousex4@gmail.com"),
            Subject = "Warning",
            Body = "<h1>" + mail.Title + "<h1>\n<h4>" + mail.Body + "</h4>",
            IsBodyHtml = true
        };
        
        message.To.Add(mail.EmailAddress);
        smtpClient.Send(message);
    }

}