using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class WateringSystemDao : IWateringSystemDao
{
    private readonly Context _context;

    public WateringSystemDao(Context context)
    {
        _context = context;
    }

    public async Task<ValveStateDto> CreateAsync(ValveState valveState)
    {
        EntityEntry<ValveState> entity = await _context.ValveState.AddAsync(valveState);
        await _context.SaveChangesAsync();
        ValveStateDto dto = new ValveStateDto()
        {
            Toggle = entity.Entity.Toggle
        };
        return dto;
    }

    public async Task<ValveStateDto> GetAsync()
    {
        IQueryable<ValveState> tempQuery= _context.ValveState.AsQueryable();
        ValveStateDto result = tempQuery.Select(v => new ValveStateDto() { Toggle = v.Toggle }).FirstOrDefault();
        return result;
        //missing await
    }
}