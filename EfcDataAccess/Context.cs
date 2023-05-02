
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public class Context : DbContext
{
	public DbSet<Humidity> Humidities { get; set; }
	public DbSet<Temperature> Temperatures { get; set; }
	public DbSet<CO2> CO2s { get; set; }
	public DbSet<Schedule> Schedules { get; set; }
	public DbSet<Interval> Intervals { get; set; }
	public DbSet<ValveState> ValveState { get; set; }

	public Context()
	{

	}

	public Context(DbContextOptions<Context> options) : base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// var dataSource = Path.Combine(Environment.CurrentDirectory, "../EfcDataAccess/Greenhouse.db");
		// optionsBuilder.UseSqlite($"Data Source = {dataSource};");
		var dataSource = optionsBuilder.UseSqlite($"Data Source = /Users/antondurcak/Documents/via/SemesterIV/SEP/greenhouse/greenhouse-backend/EfcDataAccess/Greenhouse.db");
	}

}
