using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoRepository.Core
{
	public sealed class SessionDatabase : IMongoDatabase
	{
		private readonly IMongoDatabase _database;
		private readonly MongoTransactionSession _transactionSession;

		public IMongoClient Client => throw new NotImplementedException();
		public DatabaseNamespace DatabaseNamespace => throw new NotImplementedException();
		public MongoDatabaseSettings Settings => throw new NotImplementedException();

		internal SessionDatabase(
			IMongoDatabase database,
			MongoTransactionSession transactionSession)
		{
			_database = database;
			_transactionSession = transactionSession;
		}


		public void CreateCollection(string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default)
			=> _database.CreateCollection(name, options, cancellationToken);

		public void CreateCollection(IClientSessionHandle session, string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task CreateCollectionAsync(string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default)
			=> _database.CreateCollectionAsync(name, options, cancellationToken);

		public Task CreateCollectionAsync(IClientSessionHandle session, string name, CreateCollectionOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void CreateView<TDocument, TResult>(string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void CreateView<TDocument, TResult>(IClientSessionHandle session, string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task CreateViewAsync<TDocument, TResult>(string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task CreateViewAsync<TDocument, TResult>(IClientSessionHandle session, string viewName, string viewOn, PipelineDefinition<TDocument, TResult> pipeline, CreateViewOptions<TDocument> options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void DropCollection(string name, CancellationToken cancellationToken = default)
			=> _database.DropCollection(name, cancellationToken);

		public void DropCollection(IClientSessionHandle session, string name, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task DropCollectionAsync(string name, CancellationToken cancellationToken = default)
			=> _database.DropCollectionAsync(name, cancellationToken);

		public Task DropCollectionAsync(IClientSessionHandle session, string name, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IMongoCollection<TDocument> GetCollection<TDocument>(string name, MongoCollectionSettings settings = null)
		{
			var collection = _database.GetCollection<TDocument>(name, settings);
			return new SessionCollection<TDocument>(collection, _transactionSession);
		}

		public IAsyncCursor<string> ListCollectionNames(ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<string> ListCollectionNames(IClientSessionHandle session, ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<string>> ListCollectionNamesAsync(ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<string>> ListCollectionNamesAsync(IClientSessionHandle session, ListCollectionNamesOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<BsonDocument> ListCollections(ListCollectionsOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<BsonDocument> ListCollections(IClientSessionHandle session, ListCollectionsOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync(ListCollectionsOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<BsonDocument>> ListCollectionsAsync(IClientSessionHandle session, ListCollectionsOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void RenameCollection(string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public void RenameCollection(IClientSessionHandle session, string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task RenameCollectionAsync(string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task RenameCollectionAsync(IClientSessionHandle session, string oldName, string newName, RenameCollectionOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public TResult RunCommand<TResult>(Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public TResult RunCommand<TResult>(IClientSessionHandle session, Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<TResult> RunCommandAsync<TResult>(Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<TResult> RunCommandAsync<TResult>(IClientSessionHandle session, Command<TResult> command, ReadPreference readPreference = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IAsyncCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IAsyncCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<BsonDocument>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public IMongoDatabase WithReadConcern(ReadConcern readConcern)
		{
			throw new NotImplementedException();
		}

		public IMongoDatabase WithReadPreference(ReadPreference readPreference)
		{
			throw new NotImplementedException();
		}

		public IMongoDatabase WithWriteConcern(WriteConcern writeConcern)
		{
			throw new NotImplementedException();
		}
	}
}
