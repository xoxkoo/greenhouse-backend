using Application.DaoInterfaces;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using ArgumentException = System.ArgumentException;

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

    public async Task<IEnumerable<IntervalDto>> CreateAsync(IEnumerable<IntervalDto> dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto), "Provided data cannot be null");
        }
        if (!dto.Any())
        {
            throw new ArgumentException("At least one interval must be provided", nameof(dto));
        }


        List<Interval> intervals = dto.Select(i => new Interval
        {
            DayOfWeek = i.DayOfWeek,
            EndTime = i.EndTime,
            StartTime = i.StartTime
        }).ToList();


        foreach (var interval in intervals)
        {
	        ValidateInterval(interval);
        }

        for (int i = 0; i < intervals.Count; i++)
        {
	        if (IntervalsOverlap(await GetAsync(), dto.ToList()[i]))
		        throw new ArgumentException("Intervals cannot overlap!");

	        if (IntervalsOverlap(dto, dto.ToList()[i]))
		        throw new ArgumentException("Intervals cannot overlap!");

	    }

        return await _scheduleDao.CreateAsync(intervals);
    }

    public async Task<IEnumerable<IntervalDto>> GetAsync()
    {
        return await _scheduleDao.GetAsync();
    }

    public async Task<IEnumerable<IntervalToSendDto>> GetScheduleForDay(DayOfWeek dayOfWeek)
    {
        return await _scheduleDao.GetScheduleForDay(dayOfWeek);
    }

    public async Task PutAsync(IntervalDto dto)
    {
	    ValidateInterval(new Interval() {DayOfWeek = dto.DayOfWeek, StartTime = dto.StartTime, EndTime = dto.EndTime});

	    var intervalsInDatabase = await GetAsync();

	    if (IntervalsOverlap(intervalsInDatabase, dto))
		    throw new ArgumentException("Intervals cannot overlap!");


	    await _scheduleDao.PutAsync(dto);
    }


    private void ValidateInterval(Interval interval)
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

    private bool IntervalsOverlap(IEnumerable<IntervalDto> dbIntervals, IntervalDto newInterval)
    {
	    foreach (var i in dbIntervals)
	    {
		    // exclude checking for interval that is updated
		    if (i.Id != newInterval.Id)
		    {

			    Console.WriteLine(newInterval.EndTime + " " + newInterval.StartTime);
			    Console.WriteLine(i.EndTime + " " + i.StartTime);
			    Console.WriteLine();
			    if (i.DayOfWeek == newInterval.DayOfWeek &&
			        newInterval.StartTime < i.EndTime &&
			        newInterval.StartTime > i.StartTime)
			    {
				    return true;
			    }

			    if (i.DayOfWeek == newInterval.DayOfWeek &&
			        newInterval.EndTime < i.EndTime &&
			        newInterval.EndTime > i.StartTime)
			    {
				    return true;
			    }

		    }
	    }

	    return false;
    }

    public async Task DeleteAsync(int id)
    {
        IntervalDto intervalDto = await _scheduleDao.GetByIdAsync(id);
        if (intervalDto == null)
        {
            throw new Exception($"Interval with id {id} was not found");
        }
        await _scheduleDao.DeleteAsync(id);
    }

    private async Task<List<Interval>> CheckForIntervalsInDatabase(List<Interval> intervals)
    {

	    // old intervals in database
	    IEnumerable<IntervalDto> intervalsInDatabase = await GetAsync();

	    List<Interval> intervalsToRemove = new List<Interval>();



	    for (int i = 0; i < intervals.Count; i++)
	    {
		    for (int j = i + 1; j < intervals.Count; j++)
		    {
			    if (intervals[i].DayOfWeek == intervals[j].DayOfWeek &&
			        intervals[i].StartTime < intervals[j].EndTime &&
			        intervals[j].StartTime < intervals[i].EndTime)
			    {
				    throw new ArgumentException("Intervals in the input list cannot overlap");
			    }
		    }

		    foreach (var oldInterval in intervalsInDatabase)
		    {
			    if (intervals[i].DayOfWeek == oldInterval.DayOfWeek &&
			        intervals[i].StartTime < oldInterval.EndTime &&
			        oldInterval.StartTime < intervals[i].EndTime)
			    {
				    throw new ArgumentException("Interval conflicts with an existing interval in the database");
			    }
		    }
	    }

	    return intervalsToRemove;
    }
}
