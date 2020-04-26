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
		Task CommitTransaction();
		Task AbortTransaction();
	}

	public sealed class MongoSessionContext : IMongoSessionContext
	{
		public IMongoDatabase Database { get; }
		private readonly MongoTransactionSession _transactionSession;

		public MongoSessionContext(
			IMongoDatabase database)
		{
			Database = new SessionDatabase(database, () => _transactionSession.GetSession());
			_transactionSession = new MongoTransactionSession(database);
		}

		public IMongoCollection<TDocument> GetCollection<TDocument>()
			=> Database.GetCollection<TDocument>(CollectionNameMap.For<TDocument>());

		public Task CommitTransaction()
			=> _transactionSession.CommitTransaction();

		public Task AbortTransaction()
			=> _transactionSession.AbortTransaction();
	}
}
