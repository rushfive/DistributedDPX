using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DistributedDPX.Admin.UserManagement.Extensions;
using DistributedDPX.BuildingBlocks.EventBus.EventBusCore.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DistributedDPX.Admin.UserManagement
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
				.AddIntegrationEventing(Configuration);

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
