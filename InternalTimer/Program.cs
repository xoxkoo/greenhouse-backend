using InternalTimer;
using Quartz;
using Quartz.Impl;

class Program
{
	static async Task Main(string[] args)
	{
		// create a Quartz scheduler
		var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

		// start the scheduler
		await scheduler.Start();

		// define a job
		var job = JobBuilder.Create<SchedulePlan>()
			.WithIdentity("myJob")
			.Build();

		// define a trigger that fires every day at specific time
		string runAtHours = DateTime.Now.Hour.ToString();
		string runAtMinutes = DateTime.Now.AddMinutes(1).Minute.ToString();

		var trigger = TriggerBuilder.Create()
			.WithIdentity("myTrigger")
			.WithCronSchedule($@"0 {runAtMinutes} {runAtHours} * * ?") // fire every day at specific time
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
