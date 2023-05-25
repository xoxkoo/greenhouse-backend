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

    public async Task<EmailDto> CreateAsync(Email email)
    {
        if (email == null)
        {
            throw new ArgumentNullException(nameof(email), "Mail data cannot be null.");
        }
    
        // Check if an email with the same email address already exists
        Email existingEmail = await _context.Mails.FirstOrDefaultAsync();
        if (existingEmail != null)
        {
            // Delete the existing email
            _context.Mails.Remove(existingEmail);
        }
    
        // Add the new email to the database
        EntityEntry<Email> entity = await _context.Mails.AddAsync(email);
        await _context.SaveChangesAsync();
    
        return new EmailDto()
        {
            Email = entity.Entity.EmailAddress
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
            Email = email.EmailAddress
        };
    }
}