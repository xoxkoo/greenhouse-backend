using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebAPI;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
		// Add your other service configurations

		// Add the database context
		services.AddDbContext<Context>(options =>
			options.UseSqlite(Configuration.GetConnectionString("YourConnectionString")));

		// Apply database migrations
		using var scope = services.BuildServiceProvider().CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
		dbContext.Database.Migrate();
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, IHostingEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}
		else
		{
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();
		app.UseMvc();
	}

}
