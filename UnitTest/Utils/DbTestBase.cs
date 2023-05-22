using Domain.Entities;
using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Utils;

public class DbTestBase
{
	public Context DbContext { get; private set; }

	[TestInitialize]
	public virtual void TestInit()
	{
		// configure an in memory database
		DbContext = new DbContextInMemory();
		// this ensures that the database is created in the memory and it's ready to use
		DbContext.Database.EnsureCreated();
	}


	private class DbContextInMemory : Context
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
		}
	}
}
