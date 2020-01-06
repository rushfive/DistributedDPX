using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;

namespace Participant.API
{
	public class Program
	{
		public static readonly string Namespace = typeof(Program).Namespace;
		public static readonly string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);


		public static int Main(string[] args)
		{
			var configuration = GetConfiguration();

			Log.Logger = CreateSerilogLogger(configuration);

			try
			{
				Log.Information("Configuring web host ({ApplicationContext})...", AppName);
				IWebHost host = CreateHostBuilder(configuration, args);

				//var mongo = host.Services.GetRequiredService<IMongoDbContext>();
				////List<string> collectionsCursor = mongo.GetDatabase().ListCollectionNames().Current.ToList();
				//List<string> collectionsCursor = mongo.GetDatabase().ListCollectionNamesAsync().Result.ToListAsync().Result;

				//var collection = mongo.GetDatabase().GetCollection<BsonDocument>("TestCollection");
				//var doc = new BsonDocument
				//{
				//	{ "_id", 1 },
				//	{ "TestProp", "Hello" }
				//};
				//collection.InsertOne(doc);

				//var fetched = collection.Find(Builders<BsonDocument>.Filter.Eq(d => d["_id"], 1));

				Log.Information("Starting web host ({ApplicationContext})...", AppName);
				host.Run();

				return 0;
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static IWebHost CreateHostBuilder(IConfiguration configuration, string[] args) =>
		   WebHost.CreateDefaultBuilder(args)
			   .UseConfiguration(configuration)
			   .CaptureStartupErrors(false)
			   .ConfigureKestrel(options =>
			   {
				   var ports = GetDefinedPorts(configuration);
				   options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
				   {
					   listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
				   });
				   options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
				   {
					   listenOptions.Protocols = HttpProtocols.Http2;
				   });

			   })
			   .UseStartup<Startup>()
			   //.UseApplicationInsights()
			   .UseContentRoot(Directory.GetCurrentDirectory())
			   //.UseWebRoot("Pics")
			   .UseSerilog()
			   .Build();

		private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
		{
			var seqServerUrl = configuration["Serilog:SeqServerUrl"];
			var logstashUrl = configuration["Serilog:LogstashUrl"];
			return new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.Enrich.WithProperty("ApplicationContext", AppName)
				.Enrich.FromLogContext()
				.WriteTo.Console()
				//.WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
				//.WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://logstash:8080" : logstashUrl)
				.ReadFrom.Configuration(configuration)
				.CreateLogger();
		}

		private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
		{
			var grpcPort = config.GetValue("GRPC_PORT", 81);
			var port = config.GetValue("PORT", 80);
			return (port, grpcPort);
		}

		private static IConfiguration GetConfiguration()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddEnvironmentVariables();

			return builder.Build();
		}
	}
}
