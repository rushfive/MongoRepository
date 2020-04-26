using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public sealed class SessionCollection<TDocument> : IMongoCollection<TDocument>
	{
		private readonly IMongoCollection<TDocument> _collection;
		private readonly MongoTransactionSession _transactionSession;

		public CollectionNamespace CollectionNamespace => throw new NotImplementedException();
		public IMongoDatabase Database => throw new NotImplementedException();
		public IBsonSerializer<TDocument> DocumentSerializer => throw new NotImplementedException();
		public IMongoIndexManager<TDocument> Indexes => throw new NotImplementedException();
		public MongoCollectionSettings Settings => throw new NotImplementedException();

		internal SessionCollection(
			IMongoCollection<TDocument> collection,
			MongoTransactionSession transactionSession)
		{
			_collection = collection;
			_transactionSession = transactionSession;
		}

		public IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.Aggregate(_transactionSession.GetSession(), pipeline, options, cancellationToken);

		public IAsyncCursor<TResult> Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.AggregateAsync(_transactionSession.GetSession(), pipeline, options, cancellationToken);

		public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public BulkWriteResult<TDocument> BulkWrite(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.BulkWrite(_transactionSession.GetSession(), requests, options, cancellationToken);

		public BulkWriteResult<TDocument> BulkWrite(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<BulkWriteResult<TDocument>> BulkWriteAsync(IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.BulkWriteAsync(_transactionSession.GetSession(), requests, options, cancellationToken);

		public Task<BulkWriteResult<TDocument>> BulkWriteAsync(IClientSessionHandle session, IEnumerable<WriteModel<TDocument>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public long Count(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.Count(_transactionSession.GetSession(), filter, options, cancellationToken);

		public long Count(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<long> CountAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.CountAsync(_transactionSession.GetSession(), filter, options, cancellationToken);

		public Task<long> CountAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public long CountDocuments(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.CountDocuments(_transactionSession.GetSession(), filter, options, cancellationToken);

		public long CountDocuments(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<long> CountDocumentsAsync(FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.CountDocumentsAsync(_transactionSession.GetSession(), filter, options, cancellationToken);

		public Task<long> CountDocumentsAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, CountOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public DeleteResult DeleteMany(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public DeleteResult DeleteMany(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default)
			=> _collection.DeleteMany(_transactionSession.GetSession(), filter, options, cancellationToken);

		public DeleteResult DeleteMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<DeleteResult> DeleteManyAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default)
			=> _collection.DeleteManyAsync(_transactionSession.GetSession(), filter, options, cancellationToken);

		public Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public DeleteResult DeleteOne(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public DeleteResult DeleteOne(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default)
			=> _collection.DeleteOne(_transactionSession.GetSession(), filter, options, cancellationToken);

		public DeleteResult DeleteOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<DeleteResult> DeleteOneAsync(FilterDefinition<TDocument> filter, DeleteOptions options, CancellationToken cancellationToken = default)
			=> _collection.DeleteOneAsync(_transactionSession.GetSession(), filter, options, cancellationToken);

		public Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, DeleteOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<TField> Distinct<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.Distinct(_transactionSession.GetSession(), field, filter, options, cancellationToken);

		public IAsyncCursor<TField> Distinct<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.DistinctAsync(_transactionSession.GetSession(), field, filter, options, cancellationToken);

		public Task<IAsyncCursor<TField>> DistinctAsync<TField>(IClientSessionHandle session, FieldDefinition<TDocument, TField> field, FilterDefinition<TDocument> filter, DistinctOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public long EstimatedDocumentCount(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<long> EstimatedDocumentCountAsync(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindAsync(_transactionSession.GetSession(), filter, options, cancellationToken);

		public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public TProjection FindOneAndDelete<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindOneAndDelete(_transactionSession.GetSession(), filter, options, cancellationToken);

		public TProjection FindOneAndDelete<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindOneAndDeleteAsync(_transactionSession.GetSession(), filter, options, cancellationToken);

		public Task<TProjection> FindOneAndDeleteAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOneAndDeleteOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public TProjection FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindOneAndReplace(_transactionSession.GetSession(), filter, replacement, options, cancellationToken);

		public TProjection FindOneAndReplace<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindOneAndReplaceAsync(_transactionSession.GetSession(), filter, replacement, options, cancellationToken);

		public Task<TProjection> FindOneAndReplaceAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public TProjection FindOneAndUpdate<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindOneAndUpdate(_transactionSession.GetSession(), filter, update, options, cancellationToken);

		public TProjection FindOneAndUpdate<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindOneAndUpdateAsync(_transactionSession.GetSession(), filter, update, options, cancellationToken);

		public Task<TProjection> FindOneAndUpdateAsync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<TProjection> FindSync<TProjection>(FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
			=> _collection.FindSync(_transactionSession.GetSession(), filter, options, cancellationToken);

		public IAsyncCursor<TProjection> FindSync<TProjection>(IClientSessionHandle session, FilterDefinition<TDocument> filter, FindOptions<TDocument, TProjection> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void InsertMany(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.InsertMany(_transactionSession.GetSession(), documents, options, cancellationToken);

		public void InsertMany(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task InsertManyAsync(IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.InsertManyAsync(_transactionSession.GetSession(), documents, options, cancellationToken);

		public Task InsertManyAsync(IClientSessionHandle session, IEnumerable<TDocument> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void InsertOne(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.InsertOne(_transactionSession.GetSession(), document, options, cancellationToken);

		public void InsertOne(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task InsertOneAsync(TDocument document, CancellationToken _cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task InsertOneAsync(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.InsertOneAsync(_transactionSession.GetSession(), document, options, cancellationToken);

		public Task InsertOneAsync(IClientSessionHandle session, TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<TResult> MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
			=> _collection.MapReduce(_transactionSession.GetSession(), map, reduce, options, cancellationToken);

		public IAsyncCursor<TResult> MapReduce<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
			=> _collection.MapReduceAsync(_transactionSession.GetSession(), map, reduce, options, cancellationToken);

		public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<TDocument, TResult> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>() where TDerivedDocument : TDocument
			=> _collection.OfType<TDerivedDocument>();

		public ReplaceOneResult ReplaceOne(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.ReplaceOne(_transactionSession.GetSession(), filter, replacement, options, cancellationToken);

		public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.ReplaceOneAsync(_transactionSession.GetSession(), filter, replacement, options, cancellationToken);

		public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, TDocument replacement, UpdateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public UpdateResult UpdateMany(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.UpdateMany(_transactionSession.GetSession(), filter, update, options, cancellationToken);

		public UpdateResult UpdateMany(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<UpdateResult> UpdateManyAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.UpdateManyAsync(_transactionSession.GetSession(), filter, update, options, cancellationToken);

		public Task<UpdateResult> UpdateManyAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public UpdateResult UpdateOne(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.UpdateOne(_transactionSession.GetSession(), filter, update, options, cancellationToken);

		public UpdateResult UpdateOne(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<UpdateResult> UpdateOneAsync(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
			=> _collection.UpdateOneAsync(_transactionSession.GetSession(), filter, update, options, cancellationToken);

		public Task<UpdateResult> UpdateOneAsync(IClientSessionHandle session, FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, UpdateOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<TDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IMongoCollection<TDocument> WithReadConcern(ReadConcern readConcern)
		{
			throw new NotImplementedException();
		}

		public IMongoCollection<TDocument> WithReadPreference(ReadPreference readPreference)
		{
			throw new NotImplementedException();
		}

		public IMongoCollection<TDocument> WithWriteConcern(WriteConcern writeConcern)
		{
			throw new NotImplementedException();
		}
	}
}
