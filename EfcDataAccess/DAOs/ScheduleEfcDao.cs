using System.Collections;
using Application.DaoInterfaces;
using Domain.DTOs;
using Domain.DTOs.ScheduleDTOs;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<ScheduleDto>> GetAsync()
    {
        List<IntervalDto> intervalDtos = await _context.Intervals.Select(i => new IntervalDto
        {
            DayOfWeek = i.DayOfWeek,
            StartTime = i.StartTime,
            EndTime = i.EndTime
        }).ToListAsync() ?? new List<IntervalDto>();

        IEnumerable<ScheduleDto> result = await _context.Schedules.AsQueryable()
            .Select(s => new ScheduleDto() { Intervals = intervalDtos, Id = s.Id })
            .ToListAsync();
        return result;
    }


    public async Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek)
    {
        IEnumerable<IntervalDto> result = await _context.Intervals
            .Where(i => i.DayOfWeek == dayOfWeek)
            .Select(i => new IntervalDto()
            {
                StartTime = i.StartTime,
                EndTime = i.EndTime
            })
            .ToListAsync();

        List<IntervalToSendDto> filteredIntervals = new List<IntervalToSendDto>();
        foreach (IntervalDto interval in result)
        {
            IntervalToSendDto dto = new IntervalToSendDto
            {
                StartTime = interval.StartTime,
                EndTime = interval.EndTime
            };
            filteredIntervals.Add(dto);
        }
        return filteredIntervals;
    }
}
