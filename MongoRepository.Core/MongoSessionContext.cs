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
	}

	public sealed class MongoSessionContext : IMongoSessionContext, IDisposable
	{
		public IMongoDatabase Database { get; }
		public readonly MongoTransactionSession TransactionSession;
		private bool _disposed;

		public MongoSessionContext(
			IMongoDatabase database)
		{
			Database = new SessionDatabase(database, () => TransactionSession.GetSession());
			TransactionSession = new MongoTransactionSession(database);
		}

		public IMongoCollection<TDocument> GetCollection<TDocument>()
			=> Database.GetCollection<TDocument>(CollectionNameMap.For<TDocument>());

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
