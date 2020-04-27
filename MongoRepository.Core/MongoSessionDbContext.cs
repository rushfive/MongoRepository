using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public abstract class MongoSessionDbContext : IUnitOfWork, IDisposable
	{
		protected readonly MongoSessionContext _sessionContext;
		protected readonly IMongoDatabase _database;
		protected readonly MongoSessionDbContextOptions _options;
		protected Action _onCommitCallback;
		private bool _disposed;

		//protected MongoSessionDbContext(IMongoDatabase database)
		//	: this(database, null)
		//{
		//}

		protected MongoSessionDbContext(
			IMongoDatabase database,
			MongoSessionDbContextOptions options)
		{
			_sessionContext = new MongoSessionContext(database);
			_database = _sessionContext.Database;
			_options = options ?? new MongoSessionDbContextOptions();
		}

		// may need to implement retry logic. 
		// referencing mongo's c# driver's transaction executor is a good place to start:
		// src\MongoDB.Driver\TransactionExecutor.cs (method CommitWithRetriesAsync)
		public async Task<CommitTransactionResult> Commit()
		{
			try
			{
				if (!_sessionContext.TransactionSession.IsInRunningState())
				{
					return new CommitTransactionResult.NoTransactionInProgress();
				}

				List<ICommitAggregateOperation> operations = GetOperationStores().SelectMany(s => s.GetCommitOperations()).ToList();

				foreach (var operation in operations)
				{
					await operation.ExecuteAsync(_sessionContext);
				}

				await _sessionContext.TransactionSession.CommitTransactionAsync();
				_onCommitCallback?.Invoke();

				return new CommitTransactionResult.Completed();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to commit transaction synchronously while MongoSessionDbContext was being disposed:"
						+ Environment.NewLine + ex);

				return new CommitTransactionResult.FailedWithError(ex);
			}
		}

		public Task Abort() => _sessionContext.TransactionSession.AbortTransactionAsync();

		protected abstract List<IAggregateOperationStore> GetOperationStores();

		

		public void Dispose()
		{
			if (!_disposed)
			{
				//if (_options.CommitExecution == MongoSessionDbContextOptions.CommitExecutionType.ImplicitlyInvokedOnDisposed)
				//{
				//	TryExecuteSyncCommit();
				//}

				_sessionContext.Dispose();
				_disposed = true;
			}
		}

		private void TryExecuteSyncCommit()
		{
			if (_sessionContext.TransactionSession.IsInRunningState())
			{
				try
				{
					List<ICommitAggregateOperation> operations = GetOperationStores().SelectMany(s => s.GetCommitOperations()).ToList();
					foreach (var operation in operations)
					{
						operation.Execute(_sessionContext);
					}

					_sessionContext.TransactionSession.CommitTransaction();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Failed to commit transaction synchronously while MongoSessionDbContext was being disposed:"
						+ Environment.NewLine + ex);

					_sessionContext.TransactionSession.AbortTransaction();
					throw;
				}
			}
		}
	}

	public abstract class CommitTransactionResult
	{
		public sealed class Completed : CommitTransactionResult
		{

		}

		public sealed class NoTransactionInProgress : CommitTransactionResult
		{

		}

		public sealed class FailedWithError : CommitTransactionResult
		{
			public readonly Exception Exception;

			internal FailedWithError(Exception exception)
			{
				Exception = exception;
			}
		}
	}
}
