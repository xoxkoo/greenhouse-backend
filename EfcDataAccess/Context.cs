
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public class Context : DbContext
{
	public DbSet<Humidity> Humidities { get; set; }
	public DbSet<Temperature> Temperatures { get; set; }
	public DbSet<CO2> CO2s { get; set; }

	public Context()
	{

	}
	public Context(DbContextOptions<Context> options) : base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite(@"Data Source = C:\Users\babic\RiderProjects\greenhouse-backend\EfcDataAccess\Greenhouse.db");
	}

}
