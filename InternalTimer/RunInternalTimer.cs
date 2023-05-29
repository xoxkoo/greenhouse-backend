using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using SocketServer;

namespace InternalTimer;

static class RunInternalTimer
{
	static async Task Main(string[] args)
	{

		var services = new ServiceCollection();

		// Register your services here
		services.AddScoped<Context>();
		services.AddSingleton<IScheduleDao, ScheduleEfcDao>();
		services.AddSingleton<ITemperatureDao, TemperatureEfcDao>();
		services.AddSingleton<IHumidityDao, HumidityEfcDao>();
		services.AddSingleton<ICO2Dao, CO2EfcDao>();
		services.AddSingleton<IEmailDao, EmailEfcDao>();
		services.AddSingleton<IPresetDao, PresetEfcDao>();

		services.AddScoped<IConverter, Converter>();
		services.AddScoped<ITemperatureLogic, TemperatureLogic>();
		services.AddScoped<IEmailLogic, EmailLogic>();
		services.AddScoped<IPresetLogic, PresetLogic>();
		services.AddScoped<ICO2Logic, CO2Logic>();
		services.AddScoped<IHumidityLogic, HumidityLogic>();
		services.AddScoped<IScheduleLogic, ScheduleLogic>();

		services.AddSingleton<IWebSocketServer, WebSocketServer>();

		var serviceProvider = services.BuildServiceProvider();
		var jobData = new JobDataMap
		{
			{ "scheduleLogic", serviceProvider.GetService<IScheduleLogic>() },
			{ "converter", serviceProvider.GetService<IConverter>() },
			{ "webSocketServer", serviceProvider.GetService<IWebSocketServer>() }
		};

		// create a Quartz scheduler
		var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

		// start the scheduler
		await scheduler.Start();

		// define a job
		var job = JobBuilder.Create<SchedulePlan>()
			.UsingJobData(jobData)
			// .UsingJobData("intervals", intervals)
			.WithIdentity("myJob")
			.Build();

		// define a trigger that fires every day at specific time
		// string runAtHours = DateTime.Now.Hour.ToString();
		// string runAtMinutes = DateTime.Now.Minute.ToString();
		// string runAtSeconds = DateTime.Now.AddSeconds(5).Second.ToString();

		var trigger = TriggerBuilder.Create()
			.WithIdentity("myTrigger")
			.WithCronSchedule($@"{00} {55} {23} * * ?") // fire every day at 23:55:00
			.Build();

		// schedule the job with the trigger
		await scheduler.ScheduleJob(job, trigger);

		// wait for the job to execute
		// this will wait specified number of days
		await Task.Delay(TimeSpan.FromDays(20));

		// shut down the scheduler
		await scheduler.Shutdown();
	}
}
