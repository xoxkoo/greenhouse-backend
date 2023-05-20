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
		var r = new DbTestBase();
		services.AddScoped<Context>(provider => r.DbContext);
		services.AddScoped<IPresetDao, PresetEfcDao>();
		services.AddScoped<IWebSocketServer, WebSocketServer>();
		services.AddScoped<IPresetLogic, PresetLogic>();
		services.AddScoped<IEmailLogic, EmailLogic>();
		services.AddScoped<IEmailDao, EmailEfcDao>();
		services.AddScoped<IConverter, Converter>();
		services.AddScoped<ITemperatureLogic, TemperatureLogic>();
		services.AddScoped<ITemperatureDao, TemperatureEfcDao>();
		services.AddScoped<ICO2Logic, CO2Logic>();
		services.AddScoped<ICO2Dao, CO2EfcDao>();
		services.AddScoped<IHumidityLogic, HumidityLogic>();
		services.AddScoped<IHumidityDao, HumidityEfcDao>();
		services.AddScoped<PresetController>();

	}
}
