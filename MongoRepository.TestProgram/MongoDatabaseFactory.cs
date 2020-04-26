using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;

namespace R5.MongoRepository.TestProgram
{
	static class MongoDatabaseFactory
	{
		public static IMongoDatabase Create()
		{
			var connection = new ConnectionString(Program.ConnectionString);

			if (string.IsNullOrWhiteSpace(connection.DatabaseName))
			{
				throw new InvalidOperationException("Connection string must contain database name.");
			}

			MongoClientSettings clientSettings = MongoClientSettings.FromConnectionString(connection.ToString());
			var client = new MongoClient(clientSettings);

			return client.GetDatabase(connection.DatabaseName);
		}
	}
}
