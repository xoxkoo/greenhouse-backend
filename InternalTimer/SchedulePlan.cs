using Application.LogicInterfaces;
using Quartz;

namespace InternalTimer;

public class SchedulePlan : IJob
{
	private readonly IScheduleLogic _scheduleLogic;

	public SchedulePlan(IScheduleLogic scheduleLogic)
	{
		_scheduleLogic = scheduleLogic;
	}
	public async Task Execute(IJobExecutionContext context)
	{
		Console.WriteLine($"Job executed at {DateTime.Now}");
		// var intervals = _scheduleLogic. /
	}
}
