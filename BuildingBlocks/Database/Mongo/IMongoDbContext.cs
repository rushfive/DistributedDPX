using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo
{
	public interface IMongoDbContext
	{
		Task<int> CountAsync<T>(Expression<Func<T, bool>> filter, CountOptions countOptions = null);

		Task<int> CountAsync<T>(FilterDefinition<T> filter = null, CountOptions countOptions = null);

		Task<bool> AnyAsync<T>(Expression<Func<T, bool>> filter);

		Task<bool> AnyAsync<T>(FilterDefinition<T> filter = null);

		Task<List<T>> FindAsync<T>(Expression<Func<T, bool>> filter, FindOptions<T> findOptions = null);

		Task<List<T>> FindAsync<T>(FilterDefinition<T> filter = null, FindOptions<T> findOptions = null);

		Task<IAsyncCursor<T>> FindWithCursorAsync<T>(Expression<Func<T, bool>> filter, FindOptions<T> findOptions = null);

		Task<IAsyncCursor<T>> FindWithCursorAsync<T>(FilterDefinition<T> filter = null, FindOptions<T> findOptions = null);

		Task<IAsyncCursor<TProjection>> FindWithCursorAsync<T, TProjection>(Expression<Func<T, bool>> filter, FindOptions<T, TProjection> findOptions);

		Task<IAsyncCursor<TProjection>> FindWithCursorAsync<T, TProjection>(FilterDefinition<T> filter, FindOptions<T, TProjection> findOptions);

		Task<List<TProjection>> FindAsync<T, TProjection>(FilterDefinition<T> filter = null, FindOptions<T, TProjection> findOptions = null);

		Task<List<TProjection>> FindAsync<T, TProjection>(Expression<Func<T, bool>> filter, FindOptions<T, TProjection> findOptions = null);

		Task<List<TProjection>> FindWithJoinAsync<T, TJoin, TProjection>(Expression<Func<T, object>> localField, Expression<Func<TJoin, object>> foreignField,
			Expression<Func<TProjection, object>> projectionProperty, Expression<Func<TProjection, object>> sort, FilterDefinition<TProjection> filter = null,
			int? skip = null, int? limit = null);

		Task<T> FindOneAsync<T>(Expression<Func<T, bool>> filter, FindOptions<T> findOptions = null);

		Task<T> FindOneAsync<T>(FilterDefinition<T> filter = null, FindOptions<T> findOptions = null);

		Task<TProjection> FindOneAsync<T, TProjection>(FilterDefinition<T> filter = null, FindOptions<T, TProjection> findOptions = null);

		Task<TProjection> FindOneAsync<T, TProjection>(Expression<Func<T, bool>> filter, FindOptions<T, TProjection> findOptions = null);

		Task InsertOneAsync<T>(T document, InsertOneOptions insertOptions = null);

		Task InsertManyAsync<T>(List<T> documents, InsertManyOptions insertOptions = null);

		Task<T> FindOneAndUpdateAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition, FindOneAndUpdateOptions<T> options = null);

		Task<T> FindOneAndUpdateAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, FindOneAndUpdateOptions<T> options = null);

		Task<UpdateResult> UpdateOneAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition, UpdateOptions updateOptions = null);

		Task<UpdateResult> UpdateOneAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition, UpdateOptions updateOptions = null);

		Task<ReplaceOneResult> ReplaceOneAsync<T>(FilterDefinition<T> filter, T document,
			UpdateOptions updateOptions = null);

		Task<ReplaceOneResult> ReplaceOneAsync<T>(Expression<Func<T, bool>> filter, T document,
			UpdateOptions updateOptions = null);

		Task<UpdateResult> UpdateAsync<T>(UpdateDefinition<T> updateDefinition, FilterDefinition<T> filter = null,
			UpdateOptions updateOptions = null);

		Task<UpdateResult> UpdateAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition,
			UpdateOptions updateOptions = null);

		Task<DeleteResult> DeleteOneAsync<T>(Expression<Func<T, bool>> filter);

		Task<DeleteResult> DeleteOneAsync<T>(FilterDefinition<T> filter);

		Task<DeleteResult> DeleteManyAsync<T>(Expression<Func<T, bool>> filter);

		Task<DeleteResult> DeleteManyAsync<T>(FilterDefinition<T> filter);

		Task<List<TResult>> AggregateAsync<T, TResult, TKey>(FilterDefinition<T> filter, Expression<Func<T, TKey>> groupKey, Expression<Func<IGrouping<TKey, T>, TResult>> groupValue);

		Task<List<TResult>> AggregateAsync<T, TResult>(PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null);

		Task<List<TResult>> AggregateWithJoinAsync<T, TJoin, TKey, TProjection, TResult>(
			Expression<Func<T, object>> localField, Expression<Func<TJoin, object>> foreignField, Expression<Func<TProjection, object>> asProperty,
			FilterDefinition<TProjection> filter, Expression<Func<TProjection, TKey>> groupKey, Expression<Func<IGrouping<TKey, TProjection>, TResult>> groupValue);

		//Task WithTransactionAsync(Func<IMongoDbContext, Task> wrappedCallbackAsync,
		//	ClientSessionOptions sessionOptions = null,
		//	TransactionOptions transactionOptions = null,
		//	CancellationToken cancellationToken = default);

		Task<ObjectId> AddFileAsync(string fileName, byte[] fileData);
		Task<ObjectId> AddFileAsync(string fileName, Stream fileStream);

		Task UpdateFileAsync(ObjectId id, string fileName, byte[] fileData);

		Task<byte[]> DownloadFileAsync(ObjectId id);

		Task DeleteFileAsync(ObjectId id);

		Task DropCollectionAsync(string collectionName);

		Task<(string Name, bool Rebuilt)> CreateOrRebuildIndexAsync<T>(IndexKeysDefinition<T> definition, CreateIndexOptions<T> options = null);

		Task<bool> DropIndexIfExistsAsync<T>(string name);

		IMongoClient GetClient();
		IMongoDatabase GetDatabase();
		//Task<int> GetCountByPipelineAsync<TDocument>(List<BsonDocument> pipelineStages);
	}
}
