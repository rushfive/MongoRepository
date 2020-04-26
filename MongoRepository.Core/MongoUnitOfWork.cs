using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public interface IUnitOfWork
	{
		Task Commit();
		Task Abort();
	}

	public sealed class MongoUnitOfWork : IUnitOfWork
	{
		internal readonly MongoTransactionSession _transactionSession;

		public MongoUnitOfWork(MongoTransactionSession transactionSession)
		{
			_transactionSession = transactionSession;
		}

		public Task Commit()
		{
			return _transactionSession.CommitTransaction();
		}

		public Task Abort()
		{
			return _transactionSession.AbortTransaction();
		}
	}

	public interface ISessionIdentityMaps
	{

	}
}
