using Application.Logic;
using Application.LogicInterfaces;
using Quartz;

namespace InternalTimer;

public class SchedulePlan : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		var scheduleLogic = context.JobDetail.JobDataMap.Get("scheduleLogic") as IScheduleLogic;

		Console.WriteLine($"Job executed at {DateTime.Now}");
		var intervals = await scheduleLogic.GetScheduleForDay(DateTime.Now.DayOfWeek);
		// Console.WriteLine(intervals.Count());
	}
}
