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
        
        EntityEntry<Email> entity = await _context.Mails.AddAsync(email);
        await _context.SaveChangesAsync();
        return new EmailDto()
        {
            EmailAdress = entity.Entity.EmailAddress
        };
    }

    public async Task<EmailDto> GetAsync()
    {
        var email = await _context.Mails.FirstAsync();
        return new EmailDto()
        {
            EmailAdress = email.EmailAddress
        };
    }
}