using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.Extensions.DependencyInjection;
namespace SocketClient;


static class RunWebSocket
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
		services.AddScoped<IWateringSystemDao, WateringSystemDao>();

		services.AddScoped<IConverter, Converter>();
		services.AddScoped<IValveLogic, ValveLogic>();
		services.AddScoped<ITemperatureLogic, TemperatureLogic>();
		services.AddScoped<IEmailLogic, EmailLogic>();
		services.AddScoped<IPresetLogic, PresetLogic>();
		services.AddScoped<ICO2Logic, CO2Logic>();
		services.AddScoped<IHumidityLogic, HumidityLogic>();
		services.AddScoped<IScheduleLogic, ScheduleLogic>();
		services.AddScoped<WebSocketClient>();

		var serviceProvider = services.BuildServiceProvider();

		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		WebSocketClient socket = serviceProvider.GetService<WebSocketClient>();

		try
		{

			Thread thread = new Thread( new ThreadStart(socket.Run));

			thread.Start();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw;
		}



	}

}
