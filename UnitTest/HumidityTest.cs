using System.Data.Common;
using Application.Logic;
using Application.LogicInterfaces;
using Domain.DTOs;
using Domain.Entities;
using EfcDataAccess;
using EfcDataAccess.DAOs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebAPI.Controllers;

namespace Testing;

[TestClass]
public class HumidityTest
{

	private static DbContextOptions<DbContext> dbContextOptions = new DbContextOptionsBuilder<DbContext>()
		.UseInMemoryDatabase(databaseName: "Greenhouse.db")
		.Options;

	DbContext context;
	private HumidityEfcDao dao;

	[TestInitialize]
	public void Setup()
	{
		// context = new DbContext(dbContextOptions);
		// context.Database.EnsureCreated();

		// SeedDatabase();
	}


	[ClassCleanup]
	public void CleanUp()
	{
		context.Database.EnsureDeleted();
	}
}
