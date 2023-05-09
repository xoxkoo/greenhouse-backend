using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Socket;

namespace InternalTimer;

class RunInternalTimer
{
	static async Task Main(string[] args)
	{

		var services = new ServiceCollection();

		// Register your services here
		services.AddScoped<Context>();
		services.AddSingleton<IScheduleDao, ScheduleEfcDao>();
		services.AddSingleton<ITemperatureDao, TemperatureEfcDao>();
		services.AddSingleton<IHumidityDao, HumidityEfcDao>();
		services.AddSingleton<IWateringSystemDao, WateringSystemDao>();
		services.AddSingleton<ICO2Dao, CO2EfcDao>();
		services.AddSingleton<IConverter, Converter>();
		services.AddSingleton<IScheduleLogic, ScheduleLogic>();
		services.AddSingleton<ITemperatureLogic, TemperatureLogic>();
		services.AddSingleton<ICO2Logic, CO2Logic>();
		services.AddSingleton<IHumidityLogic, HumidityLogic>();
		services.AddSingleton<IWateringSystemLogic, WateringSystemLogic>();
		services.AddSingleton<IWebSocketClient, WebSocketClient>();

		var serviceProvider = services.BuildServiceProvider();
		var jobData = new JobDataMap();
		jobData.Add("scheduleLogic", serviceProvider.GetService<IScheduleLogic>());
		jobData.Add("converter", serviceProvider.GetService<IConverter>());
		jobData.Add("webSocketClient", serviceProvider.GetService<IWebSocketClient>());

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
		string runAtHours = DateTime.Now.Hour.ToString();
		string runAtMinutes = DateTime.Now.Minute.ToString();
		string runAtSeconds = DateTime.Now.AddSeconds(5).Second.ToString();

		var trigger = TriggerBuilder.Create()
			.WithIdentity("myTrigger")
			.WithCronSchedule($@"{runAtSeconds} {runAtMinutes} {runAtHours} * * ?") // fire every day at specific time
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
