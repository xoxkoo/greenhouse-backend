using Application.DaoInterfaces;
using Domain.DTOs;
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


    public async Task<IEnumerable<IntervalDto>> CreateAsync(IEnumerable<Interval> intervals)
    {
        if (intervals == null)
        {
            throw new ArgumentNullException(nameof(intervals), "Schedule data cannot be null");
        }

        await _context.Intervals.AddRangeAsync(intervals);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save changes to database", ex);
        }

        IEnumerable<IntervalDto> intervalDtos = intervals.Select(i => new IntervalDto
        {
            Id = i.Id,
            DayOfWeek = i.DayOfWeek,
            StartTime = i.StartTime,
            EndTime = i.EndTime
        }) ?? Enumerable.Empty<IntervalDto>();

        return intervalDtos;
    }

    public async Task<IEnumerable<IntervalDto>> GetAsync()
    {
        List<IntervalDto> intervalDtos = await _context.Intervals.Select(i => new IntervalDto
        {
            Id = i.Id,
            DayOfWeek = i.DayOfWeek,
            StartTime = i.StartTime,
            EndTime = i.EndTime
        }).ToListAsync() ?? new List<IntervalDto>();
        
        return intervalDtos;
    }


    public async Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek)
    {
        IEnumerable<IntervalDto> result = await _context.Intervals
            .Where(i => i.DayOfWeek == dayOfWeek)
            .Select(i => new IntervalDto()
            {
                Id = i.Id,
                StartTime = i.StartTime,
                EndTime = i.EndTime,
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

    public async Task PutAsync(IntervalDto dto)
    {
        Interval interval = new Interval()
        {
            Id = dto.Id,
            DayOfWeek = dto.DayOfWeek,
            EndTime = dto.EndTime,
            StartTime = dto.StartTime
        };
        _context.Intervals.Update(interval);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IntervalDto> GetByIdAsync(int id)
    {
        Interval? interval= await _context.Intervals.FirstOrDefaultAsync(i => i.Id == id);

        if (interval == null)
        {
            throw new Exception($"Interval with id {id} was not found");
        }
        IntervalDto intervalDto = new IntervalDto()
        {
            Id = interval.Id,
            DayOfWeek = interval.DayOfWeek,
            StartTime = interval.StartTime,
            EndTime = interval.EndTime
        };
        return intervalDto;
    }

    public async Task DeleteAsync(int id)
    {
        Interval interval = await _context.Intervals.FirstOrDefaultAsync(i => i.Id == id);
        if (interval == null)
        {
            throw new Exception($"Interval with id {id} was not found");
        }
        _context.Intervals.Remove(interval);
        await _context.SaveChangesAsync();
    }
}
