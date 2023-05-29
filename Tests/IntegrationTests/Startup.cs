using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess.DAOs;
using Microsoft.Extensions.DependencyInjection;
using SocketServer;
using WebAPI.Controllers;

namespace Tests.IntegrationTests;

public class Startup
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddScoped<IPresetDao, PresetEfcDao>();
		services.AddScoped<IWebSocketServer, WebSocketServer>();
		services.AddScoped<IConverter, Converter>();
		services.AddScoped<ITemperatureLogic, TemperatureLogic>();
		services.AddScoped<ITemperatureDao, TemperatureEfcDao>();
		services.AddScoped<ICO2Logic, CO2Logic>();
		services.AddScoped<ICO2Dao, CO2EfcDao>();
		services.AddScoped<IHumidityLogic, HumidityLogic>();
		services.AddScoped<IValveLogic, ValveLogic>();
		services.AddScoped<IWateringSystemDao, WateringSystemDao>();
		services.AddScoped<IHumidityDao, HumidityEfcDao>();
		services.AddScoped<IEmailLogic, EmailLogic>();
		services.AddScoped<IEmailDao, EmailEfcDao>();

		// Register PresetController with its dependencies
		services.AddScoped<PresetController>(provider =>
		{
			var presetLogic = new PresetLogic(
				provider.GetService<IPresetDao>(),
				provider.GetService<IWebSocketServer>(),
				provider.GetService<IConverter>()
			);

			return new PresetController(presetLogic);
		});
	}
}