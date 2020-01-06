using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserManagement.API.Extensions;

namespace UserManagement.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services
				.AddApplicationInsightsTelemetry(Configuration)
				.AddMvcWithCors()
				.AddDbContexts(Configuration)
				.AddOptions()
				.Configure<UserManagementSettings>(Configuration)
				//.AddIntegrationEventing(Configuration)
				.AddSwaggerGen(options =>
				{
					options.DescribeAllEnumsAsStrings();
					options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
					{
						Title = "DistributedDPX - User Management HTTP API",
						Version = "v1",
						Description = "The User Management microservice HTTP API."
					});
				});

			var container = new ContainerBuilder();
			container.Populate(services);

			return new AutofacServiceProvider(container.Build());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			var settings = app.ApplicationServices.GetRequiredService<IOptions<UserManagementSettings>>().Value;


			if (!string.IsNullOrEmpty(settings.PathBase))
			{
				loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{pathBase}'", settings.PathBase);
				app.UsePathBase(settings.PathBase);
			}

			app.UseCors("CorsPolicy");
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
				endpoints.MapControllers();
				//endpoints.MapGrpcService<CatalogService>();
				//endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
				//{
				//	Predicate = _ => true,
				//	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
				//});
				//endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
				//{
				//	Predicate = r => r.Name.Contains("self")
				//});
			});

			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
			});

			ConfigureEventBus(app);
		}

		protected virtual void ConfigureEventBus(IApplicationBuilder app)
		{
			//var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
			//eventBus.Subscribe<OrderStatusChangedToAwaitingValidationIntegrationEvent, OrderStatusChangedToAwaitingValidationIntegrationEventHandler>();
			//eventBus.Subscribe<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
		}
	}
}
