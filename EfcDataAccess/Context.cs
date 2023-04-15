
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfcDataAccess;

public class Context : DbContext
{
	public DbSet<Humidity> Humidities { get; set; }
	public DbSet<Temperature> Temperatures { get; set; }
	public DbSet<CO2> CO2s { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Data Source = ../EfcDataAccess/Greenhouse.db");
	}

}
