using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public sealed class MongoTransactionSession
	{
		private readonly IMongoDatabase _database;
		private IClientSessionHandle _transactionSession;
		private static object _sessionLock = new object();

		internal static readonly TransactionOptions _transactionOptions
			= new TransactionOptions(
				ReadConcern.Snapshot, 
				ReadPreference.Primary,
				WriteConcern.WMajority);


		public MongoTransactionSession(IMongoDatabase database)
		{
			_database = database ?? throw new ArgumentNullException(nameof(database));
		}

		internal IClientSessionHandle GetSession()
		{
			if (_transactionSession != null)
			{
				return _transactionSession;
			}

			lock (_sessionLock)
			{
				if (_transactionSession == null)
				{
					var session = _database.Client.StartSession();
					session.StartTransaction(_transactionOptions);

					_transactionSession = session;
				}
			}

			return _transactionSession;
		}

		internal Task CommitTransaction()
		{
			if (!_transactionSession.IsInTransaction)
			{
				throw new InvalidOperationException("Can't commit because the session is not in a transaction state.");
			}
			return _transactionSession.CommitTransactionAsync();
		}

		internal Task AbortTransaction()
		{
			if (!_transactionSession.IsInTransaction)
			{
				throw new InvalidOperationException("Can't abort because the session is not in a transaction state.");
			}
			return _transactionSession.AbortTransactionAsync();
		}
	}
}
