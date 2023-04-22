using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Autofac;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.EntityFrameworkCore;
using Socket;

class Program
{
	static void Main(string[] args)
	{
		var builder = new ContainerBuilder();

		builder.RegisterType<Context>().AsSelf().InstancePerLifetimeScope();


		builder.RegisterType<HumidityLogic>().As<IHumidityLogic>();
		builder.RegisterType<TemperatureLogic>().As<ITemperatureLogic>();
		builder.RegisterType<CO2Logic>().As<ICO2Logic>();

		builder.RegisterType<CO2EfcDao>().As<ICO2Dao>();
		builder.RegisterType<HumidityEfcDao>().As<IHumidityDao>();
		builder.RegisterType<TemperatureEfcDao>().As<ITemperatureDao>();

		builder.RegisterType<Converter>().As<IConverter>();

		builder.RegisterType<WebSocketClient>().AsSelf();
		IContainer container = builder.Build();

		WebSocketClient socket = container.Resolve<WebSocketClient>();

		Thread thread = new Thread(async () =>
		{
			// await socket.Connect();
			// await socket.Send("Hello server!");
			await socket.Run();
		});

		thread.Start();
		// socket.Start();
	}

}
