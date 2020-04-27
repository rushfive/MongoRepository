using MongoDB.Bson;
using MongoDB.Driver;
using R5.MongoRepository.Core;
using R5.MongoRepository.TestProgram.Sample;
using R5.MongoRepository.TestProgram.Sample.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace R5.MongoRepository.TestProgram
{
	static class Program
	{
		public static readonly string ConnectionString = "mongodb://mongo1:9560,mongo2:9561/MongoRepositoryTest?replicaSet=dockerdev";

		private static IMongoDatabase _database;
		private static SampleMongoDbContext _dbContext;
		//private static IUnitOfWork _unitOfWork;

		static Program()
		{
			
			_database = MongoDatabaseFactory.Create();
			_dbContext = new SampleMongoDbContext(_database);
			CreateTestCollections();
			//var sessionDb = MongoDatabaseFactory.Create();
			//var transactionSession = new MongoTransactionSession(sessionDb);
			//_sessionDbContext = new MongoSessionContext(sessionDb, transactionSession);
			//_unitOfWork = new MongoUnitOfWork(transactionSession);


		}

		static async Task Main(string[] args)
		{

			var isaiahId = Guid.Parse("65a82960-4849-4a37-9c75-811021739869");

			Patient isaiah = await _dbContext.Patients.FindOrDefault(isaiahId);

			_dbContext.Patients.Delete(isaiah);

			await _dbContext.Commit();

			isaiah = await _dbContext.Patients.FindOrDefault(isaiahId);
			isaiah.FullName += "what";



			var test = "TEST";

			if (isaiah != null)
			{
				isaiah.FullName += "hehehehe22222";
			}



			var newPatient1 = new Patient
			{
				Id = Guid.NewGuid(),
				FullName = "Myla Lee"
			};

			_dbContext.Patients.Add(newPatient1);



			var getIsaiahAgain = await _dbContext.Patients.FindOrDefault(isaiahId);

			bool refEquals = ReferenceEquals(isaiah, getIsaiahAgain);



			await _dbContext.Commit();

			_dbContext.Patients.Add(new Patient
			{
				Id = Guid.NewGuid(),
				FullName = "PostFirstCommit PatientAdd"
			});

			await _dbContext.Commit();



			//IMongoCollection<BsonDocument> sessionCollection1 = _sessionDbContext.GetCollection<BsonDocument>("TestCollection1");
			//await sessionCollection1.InsertOneAsync(new { SessionCollection = "hi" }.ToBsonDocument());



			//await _unitOfWork.Commit();

			Console.WriteLine("Testing completed.");
			Console.ReadKey();
		}


		static void CreateTestCollections()
		{
			List<string> existing = _database.ListCollectionNames().ToList();

			var collections = new List<string> { "TestCollection1", "TestCollection2", "TestCollection3", "Appointments", "Patients" };
			foreach(var c in collections.Where(c => !existing.Contains(c)))
			{
				_database.CreateCollection(c);
			}

			WriteLine("finished creating test collections");
		}
	}
}
