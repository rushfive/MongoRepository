using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.Core
{
	public interface IMongoSessionDbContext
	{
		IMongoDatabase Database { get; }
		IMongoCollection<TDocument> GetCollection<TDocument>();
	}

	public sealed class MongoSessionDbContext : IMongoSessionDbContext
	{
		public IMongoDatabase Database { get; }

		public MongoSessionDbContext(
			IMongoDatabase database,
			MongoTransactionSession transactionSession)
		{
			Database = new SessionDatabase(database, transactionSession);
		}

		public IMongoCollection<TDocument> GetCollection<TDocument>()
			=> Database.GetCollection<TDocument>(CollectionNameMap.For<TDocument>());
	}
}
