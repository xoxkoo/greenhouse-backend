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
    private readonly IPresetDao _presetDao;
    private SmtpClient smtpClient;

    public EmailLogic(IEmailDao emailDao, IPresetDao presetDao)
    {
        _emailDao = emailDao;
        _presetDao = presetDao;
        DotNetEnv.Env.TraversePath().Load();
        smtpClient = new SmtpClient("smtp.gmail.com")
        {
	        Port = 587,
	        Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("EMAIL_USERNAME"), Environment.GetEnvironmentVariable("EMAIL_PASSWORD")),
	        EnableSsl = true,
        };
    }

    public async Task<EmailDto> CreateAsync(EmailDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Email data cannot be null");
        }
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ArgumentException("Email address cannot be empty or whitespace.", nameof(dto));
        }
        if (!dto.Email.EndsWith("@gmail.com"))
        {
            throw new ArgumentException("Email address must end with @gmail.com");
        }

        var entity = new Email()
        {
            EmailAddress = dto.Email
        };

        return await _emailDao.CreateAsync(entity);
    }

    public async Task<EmailDto> GetAsync()
    {
        return await _emailDao.GetAsync();
    }



    private void sendMail(string warning)
    {
        MailMessage message = new MailMessage
        {
            From = new MailAddress("greenhousex4@gmail.com"),
            Subject = "Warning",
            Body = @"
                <html>
                <head>
                    <style>
                        /* Define some global styles for the email */
                        body {
                            font-size: 13px;
                        }
                        h1 {
                            color: #ff0000;
                            font-size: 24px;
                            margin-bottom: 21px;
                        }
                        p {
                            margin-bottom: 10px;
                        }
                    </style>
                </head>
                <body>
                    <h1>" + warning + @"</h1>
                </body>
                </html>",
            IsBodyHtml = true
        };

        message.To.Add(_emailDao.GetAsync().Result.Email);
        smtpClient.Send(message);
    }

    public async Task CheckIfInRange(float temperature, int humidity, int co2)
    {
        SearchPresetParametersDto parametersDto = new SearchPresetParametersDto(null, true);
        var list = await _presetDao.GetAsync(parametersDto);
        PresetDto? currentPreset = list.FirstOrDefault();

        if (currentPreset == null)
        {
            return;
        }

        var thresholds = currentPreset.Thresholds;

        CheckThresholdValue("temperature", temperature, thresholds.FirstOrDefault(t => t.Type == "temperature"));
        CheckThresholdValue("humidity", humidity, thresholds.FirstOrDefault(t => t.Type == "humidity"));
        CheckThresholdValue("co2", co2, thresholds.FirstOrDefault(t => t.Type == "co2"));
    }

    private void CheckThresholdValue(string valueType, float value, ThresholdDto? threshold)
    {
        if (threshold == null)
        {
            throw new Exception($"No threshold found for {valueType}");
        }

        if (threshold.Min > value)
        {
            sendMail($"{threshold.Type} is too low");
        }
        else if (threshold.Max < value)
        {
            sendMail($"{valueType} is too high");
        }
    }
}
