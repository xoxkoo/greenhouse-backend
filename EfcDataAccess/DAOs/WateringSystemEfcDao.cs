using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
        ValveState existingState = await _context.ValveState.FirstOrDefaultAsync();
        if (existingState != null)
        {
            if (existingState.Toggle == valveState.Toggle)
            {
                throw new Exception($"The valve is already {valveState.Toggle}");
            }
            else
            {
                _context.ValveState.Remove(existingState); // remove the existing entity
                await _context.SaveChangesAsync();
                EntityEntry<ValveState> entity = await _context.ValveState.AddAsync(valveState); // add the new entity
                await _context.SaveChangesAsync();
                ValveStateDto dto = new ValveStateDto()
                {
                    Toggle = entity.Entity.Toggle
                };
                return dto;
            }
        }
        else
        {
            EntityEntry<ValveState> entity = await _context.ValveState.AddAsync(valveState);
            await _context.SaveChangesAsync();
            ValveStateDto dto = new ValveStateDto()
            {
                Toggle = entity.Entity.Toggle
            };
            return dto;
        }
    }




    public async Task<ValveStateDto> GetAsync()
    {
        IQueryable<ValveState> tempQuery = _context.ValveState.AsQueryable();
        ValveStateDto? resultDto = await tempQuery
            .Select(v => new ValveStateDto() { Toggle = v.Toggle })
            .FirstOrDefaultAsync();
        return resultDto ?? new ValveStateDto(); // return a new instance of ValveStateDto if resultDto is null
    }

}