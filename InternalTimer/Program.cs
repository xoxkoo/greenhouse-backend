using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Autofac;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using InternalTimer;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Simpl;

class Program
{
	static async Task Main(string[] args)
	{

		var services = new ServiceCollection();
		// Add required services to the service collection
		services.AddSingleton<IScheduleLogic, ScheduleLogic>();

		var serviceProvider = services.BuildServiceProvider();
		var schedulerFactory = serviceProvider.GetService<ISchedulerFactory>();
		var scheduler = await schedulerFactory.GetScheduler();

		var scheduleLogic = new JobDataMap();
		scheduleLogic.Add("scheduleLogic", serviceProvider.GetService<IScheduleLogic>());


		// start the scheduler
		await scheduler.Start();

		// define a job
		var job = JobBuilder.Create<SchedulePlan>()
			.UsingJobData(scheduleLogic)
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
