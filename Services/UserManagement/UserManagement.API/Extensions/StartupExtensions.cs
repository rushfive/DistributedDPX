using Autofac;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UserManagement.API.Infrastructure;

namespace UserManagement.API.Extensions
{
	public static class StartupExtensions
	{
		public static IServiceCollection AddMvcWithCors(this IServiceCollection services)
		{
			return services
				.AddControllers()
				.Services
				.AddCors(options =>
				{
					options.AddPolicy("CorsPolicy",
						builder => builder
						.SetIsOriginAllowed((host) => true)
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials());
				});
		}

		public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
		{
			var assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

			return services
				.AddEntityFrameworkSqlServer()
				.AddDbContext<UserManagementContext>(options =>
				{
					options.UseSqlServer(configuration["SqlServerConnectionString"],
										 sqlServerOptionsAction: sqlOptions =>
										 {
											 sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
											 //sqlOptions.MigrationsAssembly("DistributedDPX.Admin.UserManagement");
											 //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
											 sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
										 });
				});
				//.AddDbContext<IntegrationEventLogContext>(options =>
				//{
				//	options.UseSqlServer(configuration["SqlServerConnectionString"],
				//						 sqlServerOptionsAction: sqlOptions =>
				//						 {
				//							 sqlOptions.MigrationsAssembly("DistributedDPX.Admin.UserManagement");
				//							 //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
				//							 sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
				//						 });
				//});
		}

		//public static IServiceCollection AddIntegrationEventing(this IServiceCollection services, IConfiguration configuration)
		//{
		//	return services
		//		.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
		//				sp => (DbConnection c) => new IntegrationEventLogService(c))
		//		.AddTransient<IUserManagementEventService, UserManagementEventService>()
		//		.AddSingleton<IRabbitMQPersistentConnection>(sp =>
		//		{
		//			EventBusSettings settings = sp.GetRequiredService<IOptions<UserManagementSettings>>().Value.EventBus;
		//			var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();

		//			var factory = new ConnectionFactory
		//			{
		//				HostName = settings.Connection,
		//				DispatchConsumersAsync = true
		//			};

		//			if (!string.IsNullOrEmpty(settings.UserName))
		//			{
		//				factory.UserName = settings.UserName;
		//			}

		//			if (!string.IsNullOrEmpty(settings.Password))
		//			{
		//				factory.Password = settings.Password;
		//			}

		//			var retryCount = 5;
		//			if (settings.RetryCount.HasValue && settings.RetryCount > 0)
		//			{
		//				retryCount = settings.RetryCount.Value;
		//			}

		//			return new RabbitMqPersistentConnection(factory, logger, retryCount);
		//		})
		//		.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
		//		{
		//			var settings = sp.GetRequiredService<IOptions<UserManagementSettings>>().Value;

		//			var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
		//			var lifetimeScope = sp.GetRequiredService<ILifetimeScope>();
		//			var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
		//			var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

		//			EventBusSettings eventBusSettings = settings.EventBus;
		//			var retryCount = 5;
		//			if (eventBusSettings.RetryCount.HasValue && eventBusSettings.RetryCount > 0)
		//			{
		//				retryCount = eventBusSettings.RetryCount.Value;
		//			}

		//			return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, lifetimeScope, 
		//				eventBusSubcriptionsManager, settings.SubscriptionClientName, retryCount);
		//		})
		//		.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
		//}

		
	}
}
