using AutoMapper;
using LanguageExt;
using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository
{
	public abstract class MongoRepositoryDbContext : IUnitOfWork
	{
		private readonly Dictionary<Type, MongoRepository> _repositories = new Dictionary<Type, MongoRepository>();
		private readonly IMongoDatabase _database;

		protected static readonly TransactionOptions _transactionOptions
			= new TransactionOptions(ReadConcern.Snapshot, ReadPreference.Primary, WriteConcern.WMajority);

		protected MongoRepositoryDbContext(
			Dictionary<Type, MongoRepository> repositories,
			IMongoDatabase database)
		{
			_repositories = repositories;
			_database = database;
		}

		protected MongoRepository<TAggregate, TDocument, TId> GetRepository<TAggregate, TDocument, TId>()
			where TAggregate : class
			where TDocument : class
		{
			return (MongoRepository<TAggregate, TDocument, TId>)_repositories[typeof(TAggregate)];
		}

		public virtual async Task SaveChanges(bool useTransaction = true)
		{
			using (IClientSessionHandle session = await _database.Client.StartSessionAsync())
			{
				try
				{
					if (useTransaction)
					{
						session.StartTransaction(_transactionOptions);
					}

					var operations = _repositories.Values.SelectMany(r => r.GetCommitOperations());
					foreach (ICommitAggregateOperation op in operations)
					{
						// todo: bulk write?
						await op.ExecuteAsync(session);
					}

					if (useTransaction)
					{
						await session.CommitTransactionAsync();
					}

					foreach(MongoRepository repository in _repositories.Values)
					{
						repository.ClearCachedAggregateStates();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Failed to save aggregate changes to mongo:" + Environment.NewLine + ex);

					if (useTransaction)
					{
						await session.AbortTransactionAsync();
					}
					throw;
				}
			}
		}
	}
}
