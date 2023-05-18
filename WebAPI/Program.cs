using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using SocketServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWebSocketServer, WebSocketServer>();

builder.Services.AddScoped<IConverter, Converter>();
builder.Services.AddScoped<ITemperatureLogic, TemperatureLogic>();
builder.Services.AddScoped<IHumidityLogic, HumidityLogic>();
builder.Services.AddScoped<ICO2Logic, CO2Logic>();
builder.Services.AddScoped<IWateringSystemLogic, WateringSystemLogic>();
builder.Services.AddScoped<IScheduleLogic, ScheduleLogic>();
builder.Services.AddScoped<IEmailLogic, EmailLogic>();
builder.Services.AddScoped<IPresetLogic, PresetLogic>();

builder.Services.AddScoped<ITemperatureDao, TemperatureEfcDao>();
builder.Services.AddScoped<IHumidityDao, HumidityEfcDao>();
builder.Services.AddScoped<ICO2Dao, CO2EfcDao>();
builder.Services.AddScoped<IWateringSystemDao, WateringSystemDao>();
builder.Services.AddScoped<IScheduleDao, ScheduleEfcDao>();
builder.Services.AddScoped<IEmailDao, EmailEfcDao>();
builder.Services.AddScoped<IPresetDao, PresetEfcDao>();

DotNetEnv.Env.TraversePath().Load();

// Add the database context
builder.Services.AddDbContext<Context>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(x => x
	.AllowAnyMethod()
	.AllowAnyHeader()
	.SetIsOriginAllowed(origin => true)
	.AllowCredentials());


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

