using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public interface IMongoSessionContext
	{
		IMongoDatabase Database { get; }
		IMongoCollection<TDocument> GetCollection<TDocument>();
		IMongoCollection<TDocument> GetCollectionOutsideTransactionScope<TDocument>();
	}

	public sealed class MongoSessionContext : IMongoSessionContext, IDisposable
	{
		public IMongoDatabase Database { get; }
		public readonly MongoTransactionSession TransactionSession;
		private bool _disposed;
		private IMongoDatabase _originalDatabase;

		public MongoSessionContext(
			IMongoDatabase database)
		{
			Database = new SessionDatabase(database, () => TransactionSession.GetSession());
			TransactionSession = new MongoTransactionSession(database);
			_originalDatabase = database;
		}

		public IMongoCollection<TDocument> GetCollection<TDocument>()
			=> Database.GetCollection<TDocument>(CollectionNameMap.For<TDocument>());

		public IMongoCollection<TDocument> GetCollectionOutsideTransactionScope<TDocument>()
			=> _originalDatabase.GetCollection<TDocument>(CollectionNameMap.For<TDocument>());

		public void Dispose()
		{
			if (!_disposed)
			{
				TransactionSession.Dispose();
				_disposed = true;
			}
		}
	}
}
