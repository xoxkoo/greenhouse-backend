using System.Text;
using Application.DaoInterfaces;
using Application.Logic;
using Application.LogicInterfaces;
using Application.Services;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ITemperatureDao, TemperatureEfcDao>();
builder.Services.AddScoped<IHumidityDao, HumidityEfcDao>();
builder.Services.AddScoped<ICO2Dao, CO2EfcDao>();
builder.Services.AddScoped<IWateringSystemDao, WateringSystemDao>();
builder.Services.AddScoped<IScheduleDao, ScheduleEfcDao>();
builder.Services.AddScoped<IEmailDao, EmailEfcDao>();
builder.Services.AddScoped<IPresetDao, PresetEfcDao>();
builder.Services.AddScoped<IUserDao, UserEfcDao>();

DotNetEnv.Env.TraversePath().Load();

// Add the database context
builder.Services.AddDbContext<Context>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidAudience = builder.Configuration["Jwt:Audience"],
		ValidIssuer = builder.Configuration["Jwt:Issuer"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
