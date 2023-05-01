using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Quartz;

namespace InternalTimer;

public class SchedulePlan : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		Console.WriteLine($"[{DateTime.Now}] job executed");

		var scheduleLogic = context.JobDetail.JobDataMap.Get("scheduleLogic") as IScheduleLogic;
		var converter = context.JobDetail.JobDataMap.Get("converter") as IConverter;
		var intervals = await scheduleLogic?.GetScheduleForDay(DateTime.Now.DayOfWeek)!;
		var hexPayload = converter?.ConvertIntervalToHex(new ScheduleToSendDto(){Intervals = intervals});
		//TODO add websocket and send data to LoraWan


	}
}
