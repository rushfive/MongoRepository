using MongoDB.Bson;
using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R5.MongoRepository.TestProgram
{
	static class Program
	{
		public static readonly string ConnectionString = "mongodb://mongo1:9560,mongo2:9561/MongoRepositoryTest?replicaSet=dockerdev";

		private static IMongoDatabase _database;
		private static IMongoSessionDbContext _sessionDbContext;
		private static IUnitOfWork _unitOfWork;

		static Program()
		{
			_database = MongoDatabaseFactory.Create();

			var sessionDb = MongoDatabaseFactory.Create();
			var transactionSession = new MongoTransactionSession(sessionDb);
			_sessionDbContext = new MongoSessionDbContext(sessionDb, transactionSession);
			_unitOfWork = new MongoUnitOfWork(transactionSession);

			CreateTestCollections();
		}

		static async Task Main(string[] args)
		{
			IMongoCollection<BsonDocument> sessionCollection1 = _sessionDbContext.GetCollection<BsonDocument>("TestCollection1");
			await sessionCollection1.InsertOneAsync(new { SessionCollection = "hi" }.ToBsonDocument());



			await _unitOfWork.Commit();

			Console.WriteLine("Testing completed.");
			Console.ReadKey();
		}


		static void CreateTestCollections()
		{
			List<string> existing = _database.ListCollectionNames().ToList();

			var collections = new List<string> { "TestCollection1", "TestCollection2", "TestCollection3" };
			foreach(var c in collections.Where(c => !existing.Contains(c)))
			{
				_database.CreateCollection(c);
			}	
		}
	}
}
