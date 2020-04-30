//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace R5.MongoRepository.Core
//{
//	public sealed class MongoTransactionSession : IDisposable
//	{
//		private readonly IMongoDatabase _database;
//		private IClientSessionHandle _transactionSession;
//		private static object _sessionLock = new object();
//		private bool _disposed;

//		internal static readonly TransactionOptions _transactionOptions
//			= new TransactionOptions(
//				ReadConcern.Snapshot, 
//				ReadPreference.Primary,
//				WriteConcern.WMajority);


//		public MongoTransactionSession(IMongoDatabase database)
//		{
//			_database = database ?? throw new ArgumentNullException(nameof(database));
//		}

//		internal IClientSessionHandle GetSession()
//		{
//			if (IsInRunningState())
//			{
//				return _transactionSession;
//			}

//			lock (_sessionLock)
//			{
//				if (!IsInRunningState())
//				{
//					var session = _database.Client.StartSession();
//					session.StartTransaction(_transactionOptions);

//					_transactionSession = session;
//				}
//			}

//			return _transactionSession;
//		}

//		internal bool IsInRunningState()
//		{
//			return _transactionSession != null && _transactionSession.IsInTransaction;
//		}

//		internal Task CommitTransactionAsync()
//		{
//			if (IsInRunningState())
//			{
//				return _transactionSession.CommitTransactionAsync();
//			}
//			return Task.CompletedTask;
//		}

//		internal void CommitTransaction()
//		{
//			if (IsInRunningState())
//			{
//				_transactionSession.CommitTransaction();
//			}
//		}

//		internal Task AbortTransactionAsync()
//		{
//			if (IsInRunningState())
//			{
//				return _transactionSession.AbortTransactionAsync();
//			}
//			return Task.CompletedTask;
//		}

//		internal void AbortTransaction()
//		{
//			if (IsInRunningState())
//			{
//				_transactionSession.AbortTransaction();
//			}
//		}

//		public void Dispose()
//		{
//			if (!_disposed)
//			{
//				_transactionSession?.Dispose();
//				_disposed = true;
//			}
//		}
//	}
//}
