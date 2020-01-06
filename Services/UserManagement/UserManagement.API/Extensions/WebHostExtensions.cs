using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data.SqlClient;

namespace UserManagement.API.Extensions
{
	public static class WebHostExtensions
	{
		public static IWebHost MigrateDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> seeder = null) where TContext : DbContext
		{
			if (seeder == null)
			{
				return host;
			}

			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				var logger = services.GetRequiredService<ILogger<TContext>>();

				var context = services.GetService<TContext>();

				try
				{
					logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

					var retry = Policy.Handle<SqlException>()
						.WaitAndRetry(new TimeSpan[]
						{
							TimeSpan.FromSeconds(3),
							TimeSpan.FromSeconds(5),
							TimeSpan.FromSeconds(8)
						});

					//if the sql server container is not created on run docker compose this
					//migration can't fail for network related exception. The retry options for DbContext only 
					//apply to transient exceptions
					// Note that this is NOT applied when running some orchestrators (let the orchestrator to recreate the failing service)
					retry.Execute(() => InvokeSeeder(seeder, context, services));

					logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
				}
			}

			return host;
		}

		private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
			where TContext : DbContext
		{
			context.Database.Migrate();
			seeder(context, services);
		}
	}
}
