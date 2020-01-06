using Common.Abstractions.Dates;
using Mongo.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mongo
{
	public class MongoDbContext : IMongoDbContext
	{
		static MongoDbContext()
		{
			//Always ignore extra elements that are in mongo but not in the C# object
			var cp = new ConventionPack
			{
				new IgnoreExtraElementsConvention(true),
				new EnumRepresentationConvention(BsonType.String)
			};

			ConventionRegistry.Register("Custom Registries", cp, t => true);
		}

		private static void RegisterSerializers()
		{
			BsonSerializer.RegisterSerializer(
				new DictionaryInterfaceImplementerSerializer<Dictionary<string, object>>()
					.WithValueSerializer(new BetterObjectSerializer()));

			BsonSerializer.RegisterSerializer(new DsDateSerializer());
			BsonSerializer.RegisterSerializer(new NullableSerializer<DsDate>(new DsDateSerializer()));
			BsonSerializer.RegisterSerializer(new DsTimeSerializer());
			BsonSerializer.RegisterSerializer(new NullableSerializer<DsTime>(new DsTimeSerializer()));
			BsonSerializer.RegisterSerializer(new DsDateTimeSerializer());
			BsonSerializer.RegisterSerializer(new NullableSerializer<DsDateTime>(new DsDateTimeSerializer()));
			BsonSerializer.RegisterSerializer(new DsGuidSerializer());
			BsonSerializer.RegisterSerializer(new NullableSerializer<Guid>(new DsGuidSerializer()));
			//BsonSerializer.RegisterSerializer(new HostNameSerializer());
			//BsonSerializer.RegisterSerializer(new NullableSerializer<HostName>(new HostNameSerializer()));
			BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
			BsonSerializer.RegisterSerializer(new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
			//BsonSerializer.RegisterSerializer(new CriteriaSerializer());
			//BsonSerializer.RegisterSerializer(new ChartCriteriaSerializer());

			BsonSerializer.RegisterDiscriminator(typeof(DsDate), BsonValue.Create("Date"));
			BsonSerializer.RegisterDiscriminator(typeof(DsTime), BsonValue.Create("Time"));
			BsonSerializer.RegisterDiscriminator(typeof(DsDateTime), BsonValue.Create("DateTime"));
			BsonSerializer.RegisterSerializationProvider(new GenericSerializationProvider());
		}

		private readonly ConnectionString _connectionString;
		private readonly MongoClientSettings _settings;

		public MongoDbContext(
			ConnectionString connectionString,
			MongoClientSettings settings)
		{
			_connectionString = connectionString;
			_settings = settings;
		}

		public async Task<int> CountAsync<T>(FilterDefinition<T> filter = null, CountOptions countOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			if (filter == null)
			{
				//empty filter
				filter = new BsonDocumentFilterDefinition<T>(new BsonDocument());
			}


			return (int)await collection.CountDocumentsAsync(filter, countOptions).ConfigureAwait(false);
		}

		public async Task<int> CountAsync<T>(Expression<Func<T, bool>> filter, CountOptions countOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();



			return (int)await collection.CountDocumentsAsync(filter, countOptions).ConfigureAwait(false);
		}


		public async Task<bool> AnyAsync<T>(FilterDefinition<T> filter = null)
		{
			return await this.CountAsync(filter).ConfigureAwait(false) > 0;
		}

		public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> filter)
		{
			return await this.CountAsync(filter).ConfigureAwait(false) > 0;
		}

		public async Task<List<T>> FindAsync<T>(FilterDefinition<T> filter = null,
			FindOptions<T> findOptions = null)
		{
			IAsyncCursor<T> asyncCursor = await this.FindWithCursorAsync(filter, findOptions).ConfigureAwait(false);
			return await asyncCursor.ToListAsync().ConfigureAwait(false);
		}

		public async Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter,
			FindOptions<T> findOptions = null)
		{
			IAsyncCursor<T> asyncCursor = await this.FindWithCursorAsync(filter, findOptions).ConfigureAwait(false);
			return await asyncCursor.ToListAsync().ConfigureAwait(false);
		}

		public Task<IAsyncCursor<T>> FindWithCursorAsync<T>(FilterDefinition<T> filter = null,
			FindOptions<T> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			if (filter == null)
			{
				//empty filter
				filter = new BsonDocumentFilterDefinition<T>(new BsonDocument());
			}
			if (findOptions == null)
			{
				//default
				findOptions = new FindOptions<T>
				{
					BatchSize = 200
				};
			}

			return collection.FindAsync(filter, findOptions);
		}

		public Task<IAsyncCursor<T>> FindWithCursorAsync<T>(Expression<Func<T, bool>> filter,
			FindOptions<T> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.FindAsync(filter, findOptions);
		}

		public async Task<List<TProjection>> FindAsync<T, TProjection>(FilterDefinition<T> filter = null,
			FindOptions<T, TProjection> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();

			if (filter == null)
			{
				//empty filter
				filter = new BsonDocumentFilterDefinition<T>(new BsonDocument());
			}

			IAsyncCursor<TProjection> asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);

			return await asyncCursor.ToListAsync().ConfigureAwait(false);
		}

		public Task<IAsyncCursor<TProjection>> FindWithCursorAsync<T, TProjection>(Expression<Func<T, bool>> filter,
			FindOptions<T, TProjection> findOptions)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();



			return collection.FindAsync(filter, findOptions);
		}

		public Task<IAsyncCursor<TProjection>> FindWithCursorAsync<T, TProjection>(FilterDefinition<T> filter,
			FindOptions<T, TProjection> findOptions)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();



			return collection.FindAsync(filter, findOptions);
		}

		public async Task<List<TProjection>> FindAsync<T, TProjection>(Expression<Func<T, bool>> filter,
			FindOptions<T, TProjection> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			IAsyncCursor<TProjection> asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);

			return await asyncCursor.ToListAsync().ConfigureAwait(false);
		}

		public async Task<List<TProjection>> FindWithJoinAsync<T, TJoin, TProjection>(Expression<Func<T, object>> localField,
			Expression<Func<TJoin, object>> foreignField,
			Expression<Func<TProjection, object>> asProperty, Expression<Func<TProjection, object>> sort,
			FilterDefinition<TProjection> filter = null, int? skip = null, int? limit = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			IMongoCollection<TJoin> joinCollection = this.GetCollectionByType<TJoin>();

			IAggregateFluent<T> aggregationFluent = collection
				.Aggregate();

			IAggregateFluent<TProjection> match = aggregationFluent
				.Lookup(joinCollection, localField, foreignField, asProperty)
				.Match(filter ?? Builders<TProjection>.Filter.Empty)
				.SortBy(sort);

			if (skip.HasValue)
			{
				match = match.Skip(skip.Value);
			}

			if (limit.HasValue)
			{
				match = match.Limit(limit.Value);
			}

			return await match.ToListAsync();
		}

		public async Task<T> FindOneAsync<T>(FilterDefinition<T> filter = null, FindOptions<T> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			if (filter == null)
			{
				//empty filter
				filter = new BsonDocumentFilterDefinition<T>(new BsonDocument());
			}

			IAsyncCursor<T> asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);


			return await asyncCursor.SingleOrDefaultAsync().ConfigureAwait(false);
		}

		public async Task<T> FindOneAsync<T>(Expression<Func<T, bool>> filter, FindOptions<T> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();

			IAsyncCursor<T> asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);

			return await asyncCursor.SingleOrDefaultAsync().ConfigureAwait(false);
		}

		public async Task<TProjection> FindOneAsync<T, TProjection>(FilterDefinition<T> filter = null,
			FindOptions<T, TProjection> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();

			if (filter == null)
			{
				//empty filter
				filter = new BsonDocumentFilterDefinition<T>(new BsonDocument());
			}

			IAsyncCursor<TProjection> asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);

			return await asyncCursor.SingleOrDefaultAsync();
		}

		public async Task<TProjection> FindOneAsync<T, TProjection>(Expression<Func<T, bool>> filter,
			FindOptions<T, TProjection> findOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();

			IAsyncCursor<TProjection> asyncCursor = await collection.FindAsync(filter, findOptions).ConfigureAwait(false);

			return await asyncCursor.SingleOrDefaultAsync();
		}

		public async Task InsertOneAsync<T>(T document, InsertOneOptions insertOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			await collection.InsertOneAsync(document, insertOptions);
		}

		public async Task InsertManyAsync<T>(List<T> documents, InsertManyOptions insertOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			await collection.InsertManyAsync(documents, insertOptions);
		}

		public Task<T> FindOneAndUpdateAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition,
			FindOneAndUpdateOptions<T> options = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.FindOneAndUpdateAsync(filter, updateDefinition, options);
		}

		public Task<T> FindOneAndUpdateAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition,
			FindOneAndUpdateOptions<T> options = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.FindOneAndUpdateAsync(filter, updateDefinition, options);
		}

		public async Task<UpdateResult> UpdateOneAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition,
			UpdateOptions updateOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return await collection.UpdateOneAsync(filter, updateDefinition, updateOptions);
		}

		public Task<UpdateResult> UpdateOneAsync<T>(Expression<Func<T, bool>> filter,
			UpdateDefinition<T> updateDefinition, UpdateOptions updateOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.UpdateOneAsync(filter, updateDefinition, updateOptions);
		}

		public Task<DeleteResult> DeleteOneAsync<T>(Expression<Func<T, bool>> filter)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.DeleteOneAsync(filter);
		}

		public Task<DeleteResult> DeleteOneAsync<T>(FilterDefinition<T> filter)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.DeleteOneAsync(filter);
		}

		public Task<DeleteResult> DeleteManyAsync<T>(Expression<Func<T, bool>> filter)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.DeleteManyAsync(filter);
		}

		public Task<DeleteResult> DeleteManyAsync<T>(FilterDefinition<T> filter)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.DeleteManyAsync(filter);
		}

		public Task<ReplaceOneResult> ReplaceOneAsync<T>(FilterDefinition<T> filter, T document,
			UpdateOptions updateOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.ReplaceOneAsync(filter, document, updateOptions);
		}

		public Task<ReplaceOneResult> ReplaceOneAsync<T>(Expression<Func<T, bool>> filter, T document,
			UpdateOptions updateOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.ReplaceOneAsync(filter, document, updateOptions);
		}

		public Task<UpdateResult> UpdateAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition,
			UpdateOptions updateOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();


			return collection.UpdateManyAsync(filter, updateDefinition, updateOptions);
		}

		public Task<UpdateResult> UpdateAsync<T>(UpdateDefinition<T> updateDefinition,
			FilterDefinition<T> filter = null,
			UpdateOptions updateOptions = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			if (filter == null)
			{
				//empty filter
				filter = new BsonDocumentFilterDefinition<T>(new BsonDocument());
			}


			return collection.UpdateManyAsync(filter, updateDefinition, updateOptions);
		}

		public Task<List<TResult>> AggregateAsync<T, TResult, TKey>(FilterDefinition<T> filter,
			Expression<Func<T, TKey>> groupKey, Expression<Func<IGrouping<TKey, T>, TResult>> groupValue)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			IAggregateFluent<T> fluent = collection.Aggregate();

			return fluent
				.Match(filter)
				.Group(groupKey, groupValue)
				.ToListAsync();
		}

		public Task<List<TResult>> AggregateAsync<T, TResult>(PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();



			return collection.Aggregate(pipeline, options).ToListAsync();
		}

		public async Task<List<TResult>> AggregateWithJoinAsync<T, TJoin, TKey, TProjection, TResult>(
			Expression<Func<T, object>> localField, Expression<Func<TJoin, object>> foreignField,
			Expression<Func<TProjection, object>> asProperty,
			FilterDefinition<TProjection> filter, Expression<Func<TProjection, TKey>> groupKey,
			Expression<Func<IGrouping<TKey, TProjection>, TResult>> groupValue)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			IMongoCollection<TJoin> joinCollection = this.GetCollectionByType<TJoin>();

			IAggregateFluent<T> fluent = collection.Aggregate();

			return await fluent
				.Lookup(joinCollection, localField, foreignField, asProperty)
				.Match(filter)
				.Group(groupKey, groupValue)
				.ToListAsync();
		}

		//public Task WithTransactionAsync(Func<IMongoDbContext, Task> wrappedCallbackAsync,
		//	ClientSessionOptions sessionOptions = null,
		//	TransactionOptions transactionOptions = null,
		//	CancellationToken cancellationToken = default)
		//{
		//	// don't start a new transaction if already in one
		//	// handles nested calls to this method

		//	if (this.session != null)
		//	{
		//		//TODO is there a safe way to do this with retries? and not using specified options
		//		throw new InvalidOperationException("Cannot have nested transactions.");
		//	}

		//	return this.mongoClientProvider.RunTransactionAsync(Func, this.logger, sessionOptions, transactionOptions, cancellationToken);

		//	Task Func(IClientSessionHandle clientSession)
		//	{
		//		IMongoDbContext transactionContext = this.WithContext(clientSession: clientSession);
		//		return wrappedCallbackAsync(transactionContext);
		//	}
		//}

		private GridFSBucket GetGridFsBucket()
		{
			IMongoDatabase database = GetDatabase();
			return new GridFSBucket(database);
		}

		// GridFS
		public async Task<ObjectId> AddFileAsync(string fileName, byte[] fileData)
		{
			GridFSBucket bucket = this.GetGridFsBucket();
			return await bucket.UploadFromBytesAsync(fileName, fileData);
		}
		public async Task<ObjectId> AddFileAsync(string fileName, Stream fileStream)
		{
			GridFSBucket bucket = this.GetGridFsBucket();
			return await bucket.UploadFromStreamAsync(fileName, fileStream);
		}

		public async Task UpdateFileAsync(ObjectId id, string fileName, byte[] fileData)
		{
			GridFSBucket bucket = this.GetGridFsBucket();
			await bucket.DeleteAsync(id);
			await bucket.UploadFromBytesAsync(id, fileName, fileData);
		}

		public async Task<byte[]> DownloadFileAsync(ObjectId id)
		{
			GridFSBucket bucket = this.GetGridFsBucket();
			return await bucket.DownloadAsBytesAsync(id);
		}

		public async Task DeleteFileAsync(ObjectId id)
		{
			GridFSBucket bucket = this.GetGridFsBucket();
			await bucket.DeleteAsync(id);
		}

		private IMongoCollection<T> GetCollectionByType<T>()
		{
			var attribute = typeof(T).GetTypeInfo().GetCustomAttribute<DocumentCollectionAttribute>(true);
			if (attribute?.Name == null)
			{
				throw new Exception($"No collection for the type '{typeof(T)}'.");
			}

			string collectionName = attribute.Name;

			return GetDatabase().GetCollection<T>(collectionName);
		}

		public Task DropCollectionAsync(string collectionName)
		{
			return GetDatabase().DropCollectionAsync(collectionName);
		}

		public async Task<(string Name, bool Rebuilt)> CreateOrRebuildIndexAsync<T>(IndexKeysDefinition<T> definition, CreateIndexOptions<T> options = null)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();

			bool rebuilt = false;
			if (options?.Name != null)
			{
				rebuilt = await this.DropIndexIfExistsAsync<T>(options.Name);
			}
			var model = new CreateIndexModel<T>(definition, options);
			string name = await collection.Indexes.CreateOneAsync(model);

			return (name, rebuilt);
		}

		public async Task<bool> DropIndexIfExistsAsync<T>(string name)
		{
			IMongoCollection<T> collection = this.GetCollectionByType<T>();
			IAsyncCursor<BsonDocument> cursor = await collection.Indexes.ListAsync();

			bool rebuilt = false;
			List<BsonDocument> indexes = await cursor.ToListAsync();
			if (indexes.Any(i => i["name"] == name))
			{
				rebuilt = true;
				await collection.Indexes.DropOneAsync(name);
			}
			return rebuilt;
		}

		public IMongoClient GetClient()
		{
			return new MongoClient(_settings);
		}

		public IMongoDatabase GetDatabase()
		{
			if (string.IsNullOrWhiteSpace(_connectionString.DatabaseName))
			{
				throw new Exception("Database name must be provided.");
			}
			return GetClient().GetDatabase(_connectionString.DatabaseName);
		}

		//public async Task<int> GetCountByPipelineAsync<TDocument>(List<BsonDocument> pipelineStages)
		//{
		//	List<BsonDocument> stages = pipelineStages.ToList();
		//	if (stages.Any(s => s.Contains("$sort")))
		//	{
		//		throw new Exception("Sort stages should not be given to count aggregations");
		//	}
		//	stages.Add(new BsonDocument
		//	{
		//		{"$count", "Count"}
		//	});

		//	BsonDocumentStagePipelineDefinition<TDocument, AggregateCountDocument> countPipeline = stages.ToPipeline<TDocument, AggregateCountDocument>();
		//	List<AggregateCountDocument> countDoc = await this.AggregateAsync(countPipeline);

		//	if (!countDoc.Any())
		//	{
		//		return 0;
		//	}

		//	return countDoc.Single().Count;
		//}

		public class AggregateCountDocument
		{
			[BsonElement]
			public int Count { get; set; }
		}
	}
}
