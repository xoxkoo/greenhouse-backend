using Application.LogicInterfaces;
using Domain.DTOs;
using Quartz;
using SocketServer;

namespace InternalTimer;

public class SchedulePlan : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		Console.WriteLine($"[{DateTime.Now}] job executed");

		var scheduleLogic = context.JobDetail.JobDataMap.Get("scheduleLogic") as IScheduleLogic;
		var converter = context.JobDetail.JobDataMap.Get("converter") as IConverter;
		var socket = context.JobDetail.JobDataMap.Get("webSocketServer") as IWebSocketServer;

		try
		{
			var intervals = await scheduleLogic?.GetScheduleForDay(DateTime.Now.DayOfWeek)!;

			string? hexPayload = converter?.ConvertIntervalToHex(intervals);
			//TODO discuss if we want to use socket here or call the logic
			Console.WriteLine(hexPayload);
			socket?.Send(hexPayload);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			new Exception("Intervals were not sent to the server");
		}

	}
}
