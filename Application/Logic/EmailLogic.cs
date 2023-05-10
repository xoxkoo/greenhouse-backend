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
    private readonly IPresetDao _presetDao;

    public EmailLogic(IEmailDao emailDao, IPresetDao presetDao)
    {
        _emailDao = emailDao;
        _presetDao = presetDao;
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


    private void sendMail(Email mail)
    {
        MailMessage message = new MailMessage
        {
            From = new MailAddress("greenhousex4@gmail.com"),
            Subject = "Warning",
            Body = "<h1>" + mail.Title + "<h1>\n<h4>" + mail.Body + "</h4>",
            IsBodyHtml = true
        };
        mail.EmailAddress = _emailDao.GetAsync().Result.EmailAdress;
        message.To.Add(mail.EmailAddress);
        smtpClient.Send(message);
    }
    
    public async Task CheckIfInRange(float temperature, int humidity, int co2)
    {
        SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(null, true);
        var list = await _presetDao.GetAsync(parametersDto);
        PresetDto? currentPreset = list.FirstOrDefault();
        var thresholds = currentPreset.Thresholds;
        Threshold? temperatureThreshold = thresholds.FirstOrDefault(t => t.Type == "temperature");
        Threshold? humidityThreshold = thresholds.FirstOrDefault(t => t.Type == "humidity");
        Threshold? co2Threshold = thresholds.FirstOrDefault(t => t.Type == "co2");

        if (temperatureThreshold.MinValue > temperature)
        {
            sendMail(temperatureTooLow);
        }
        else if (temperatureThreshold.MaxValue < temperature)
        {
            sendMail(temperatureTooHigh);
        }

        if (humidityThreshold.MinValue > humidity)
        {
            sendMail(humidityTooLow);
        }
        else if (humidityThreshold.MaxValue < humidity)
        {
            sendMail(humidityTooHigh);
        }

        if (co2Threshold.MinValue > co2)
        {
            sendMail(Co2LvlTooLow);
        }
        else if (co2Threshold.MaxValue < co2)
        {
            sendMail(humidityTooHigh);
        }
    }
}