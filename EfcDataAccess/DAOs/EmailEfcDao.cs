using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class EmailEfcDao : IEmailDao
{
    private readonly Context _context;

    public EmailEfcDao(Context context)
    {
        _context = context;
    }

    public async Task<EmailDto> CreateAsync(NotificationEmail notificationEmail)
    {
        if (notificationEmail == null)
        {
            throw new ArgumentNullException(nameof(notificationEmail), "Mail data cannot be null.");
        }
    
        // Check if an email with the same email address already exists
        NotificationEmail existingNotificationEmail = await _context.Mails.FirstOrDefaultAsync();
        if (existingNotificationEmail != null)
        {
            // Delete the existing email
            _context.Mails.Remove(existingNotificationEmail);
        }
    
        // Add the new email to the database
        EntityEntry<NotificationEmail> entity = await _context.Mails.AddAsync(notificationEmail);
        await _context.SaveChangesAsync();
    
        return new EmailDto()
        {
            EmailAdress = entity.Entity.Email
        };
    }

    public async Task<EmailDto> GetAsync()
    {
        var email = await _context.Mails.FirstOrDefaultAsync();
        if (email == null)
        {
            throw new Exception("No email found.");
        }
        return new EmailDto()
        {
            EmailAdress = email.Email
        };
    }
}