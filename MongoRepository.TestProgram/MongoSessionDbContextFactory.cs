﻿using MongoDB.Driver;
using R5.MongoRepository.Core;

namespace R5.MongoRepository.TestProgram
{
	static class MongoSessionDbContextFactory
	{
		public static IMongoSessionContext Create()
		{
			IMongoDatabase database = MongoDatabaseFactory.Create();
			//var session = new MongoTransactionSession(database);

			return new MongoSessionContext(database);

		}
	}
}
