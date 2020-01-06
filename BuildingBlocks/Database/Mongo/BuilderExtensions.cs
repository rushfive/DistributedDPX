using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mongo
{
	public static class BuilderExtensions
	{
		public static IServiceCollection AddMongoDatabase(this IServiceCollection services,
			string mongoConnectionString)
		{
			services
				.AddScoped<MongoClientSettings>(sp => MongoClientSettings.FromConnectionString(mongoConnectionString))
				.AddScoped<ConnectionString>(sp => new ConnectionString(mongoConnectionString))
				.AddScoped<IMongoDbContext, MongoDbContext>();

			return services;
		}
	}
}
