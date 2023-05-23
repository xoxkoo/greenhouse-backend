using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using SocketServer;
using Testing.Utils;
using WebAPI.Controllers;

namespace Testing.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;

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
		services.AddScoped<IHumidityDao, HumidityEfcDao>();
		services.AddScoped<IEmailLogic, EmailLogic>();
		services.AddScoped<IEmailDao, EmailEfcDao>();

		// Register PresetController with its dependencies
		services.AddScoped<PresetController>(provider =>
		{
			var presetLogic = new PresetLogic(
				provider.GetService<IPresetDao>(),
				provider.GetService<IWebSocketServer>(),
				provider.GetService<Converter>()
			);

			return new PresetController(presetLogic);
		});

	}
}
