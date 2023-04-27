using System.Collections;
using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfcDataAccess.DAOs;

public class ScheduleEfcDao : IScheduleDao
{
    private readonly Context _context;

    public ScheduleEfcDao(Context context)
    {
        _context = context;
    }


    public async Task<ScheduleDto> CreateAsync(Schedule schedule)
    {
        if (schedule == null)
        {
            throw new ArgumentNullException(nameof(schedule), "Schedule data cannot be null");
        }

        EntityEntry<Schedule> entity = await _context.Schedules.AddAsync(schedule);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save changes to database", ex);
        }

        IEnumerable<IntervalDto> intervalDtos = entity.Entity.Intervals?.Select(i => new IntervalDto
        {
            DayOfWeek = i.DayOfWeek,
            StartTime = i.StartTime,
            EndTime = i.EndTime
        }) ?? Enumerable.Empty<IntervalDto>();

        return new ScheduleDto
        {
            Id = entity.Entity.Id,
            Intervals = intervalDtos
        };
    }
}