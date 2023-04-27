
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
		optionsBuilder.UseSqlite("Data Source = ./Greenhouse.db");
	}

}
