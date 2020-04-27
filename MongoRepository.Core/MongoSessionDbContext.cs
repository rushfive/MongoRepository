using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public abstract class MongoSessionDbContext : IUnitOfWork
	{
		protected readonly MongoSessionContext _sessionContext;
		protected readonly IMongoDatabase _database;

		protected MongoSessionDbContext(IMongoDatabase database)
		{
			_sessionContext = new MongoSessionContext(database);
			_database = _sessionContext.Database;
		}

		public async Task Commit()
		{
			List<ICommitAggregateOperation> operations = GetOperationStores().SelectMany(s => s.GetCommitOperations()).ToList();

			foreach (var operation in operations)
			{
				await operation.Execute(_sessionContext);
			}

			await _sessionContext.CommitTransaction();
		}

		public Task Abort() => _sessionContext.AbortTransaction();

		protected abstract List<IAggregateOperationStore> GetOperationStores();
	}
}
