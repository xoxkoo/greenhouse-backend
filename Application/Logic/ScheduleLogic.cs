using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Domain.Entities;

namespace Application.Logic;

public class ScheduleLogic : IScheduleLogic
{
    private readonly IScheduleDao _scheduleDao;
    private readonly IConverter _converter;


    public ScheduleLogic(IScheduleDao scheduleDao, IConverter converter)
    {
        _scheduleDao = scheduleDao;
        _converter = converter;
    }

    public async Task<ScheduleDto> CreateAsync(ScheduleCreationDto dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Provided data cannot be null");
        }
        if (dto.Intervals == null || !dto.Intervals.Any())
        {
            throw new ArgumentException("At least one interval must be provided", nameof(dto.Intervals));
        }
        List<Interval> intervals = dto.Intervals.Select(i => new Interval
        {
            DayOfWeek = i.DayOfWeek,
            EndTime = i.EndTime,
            StartTime = i.StartTime
        }).ToList();

        foreach (var interval in intervals)
        {
            if (interval.StartTime >= interval.EndTime)
            {
                throw new ArgumentException("The start time has to be smaller than end time for all intervals");
            }
            if (interval.StartTime.Hours >= 24 || interval.EndTime.Hours >= 24)
            {
                throw new ArgumentException("Hours value in start and end times cannot be 24 or more");
            }
            if (interval.StartTime.Minutes >= 60 || interval.EndTime.Minutes >= 60)
            {
                throw new ArgumentException("Minutes value in start and end times cannot be 60 or more");
            }
            if (interval.StartTime.Seconds >= 60 || interval.EndTime.Seconds >= 60)
            {
                throw new ArgumentException("Seconds value in start and end times cannot be 60 or more");
            }
        }

        for (int i = 0; i < intervals.Count; i++)
        {
            for (int j = i + 1; j < intervals.Count; j++)
            {
                if (intervals[i].DayOfWeek == intervals[j].DayOfWeek &&
                    intervals[i].StartTime < intervals[j].EndTime &&
                    intervals[j].StartTime < intervals[i].EndTime)
                {
                    throw new ArgumentException("Intervals cannot overlap");
                }
            }
        }

        Schedule schedule = new Schedule
        {
            Intervals = intervals
        };

        ScheduleDto scheduleDto = new ScheduleDto() { Intervals = dto.Intervals };

        string hexPayload = _converter.ConvertIntervalToHex(dto);

        //TODO call socket

        return await _scheduleDao.CreateAsync(schedule);
    }

    public async Task<IEnumerable<ScheduleDto>> GetAsync()
    {
        return await _scheduleDao.GetAsync();
    }

    public async Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek)
    {
        return await _scheduleDao.GetScheduleForDay(dayOfWeek);
    }
}
