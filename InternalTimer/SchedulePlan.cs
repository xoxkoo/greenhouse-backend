using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.DTOs.CreationDTOs;
using Quartz;
using Socket;

namespace InternalTimer;

public class SchedulePlan : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		Console.WriteLine($"[{DateTime.Now}] job executed");

		var scheduleLogic = context.JobDetail.JobDataMap.Get("scheduleLogic") as IScheduleLogic;
		var converter = context.JobDetail.JobDataMap.Get("converter") as IConverter;
		var socket = context.JobDetail.JobDataMap.Get("webSocketClient") as IWebSocketClient;

		try
		{
			var intervals = await scheduleLogic?.GetScheduleForDay(DateTime.Now.DayOfWeek)!;
			string? hexPayload = converter?.ConvertIntervalToHex(new ScheduleToSendDto(){Intervals = intervals});
			//TODO discuss if we want to use socket here or call the logic
			socket?.Send(hexPayload);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);

		}




	}
}
