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
			int maxIntervals = 5;
			var intervals = await scheduleLogic?.GetScheduleForDay(DateTime.Now.DayOfWeek)!;

			// send just one message
			// clear all previous intervals
			if (intervals.Count() <= maxIntervals)
			{
				string? hexPayload = converter?.ConvertIntervalToHex(new ScheduleToSendDto(){Intervals = intervals}, true);
				socket?.Send(hexPayload);
			}
			else
			{
				// clear all previous intervals
				var intervalsToSend = intervals.Take(maxIntervals);
				string? hexPayload = converter?.ConvertIntervalToHex(new ScheduleToSendDto() { Intervals = intervalsToSend }, true);

				// Send remaining intervals in groups of 5
				for (int i = maxIntervals; i < intervals.Count(); i += maxIntervals)
				{
					intervalsToSend = intervals.Skip(i).Take(maxIntervals);
					hexPayload = converter?.ConvertIntervalToHex(new ScheduleToSendDto { Intervals = intervalsToSend });
					socket?.Send(hexPayload);
				}
			}
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			new Exception("Intervals were not sent to the server");
		}

	}
}
