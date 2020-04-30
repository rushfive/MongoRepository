using AutoMapper;
using LanguageExt;
using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository
{
	public abstract class MongoRepositoryDbContext : IUnitOfWork
	{
		private readonly Dictionary<Type, MongoRepository> _repositories = new Dictionary<Type, MongoRepository>();

		protected MongoRepositoryDbContext(
			Dictionary<Type, MongoRepository> repositories)
		{
			_repositories = repositories;
		}

		protected MongoRepository<TAggregate, TDocument, TId> GetRepository<TAggregate, TDocument, TId>() 
			where TAggregate : class
			where TDocument : class
		{
			return (MongoRepository<TAggregate, TDocument, TId>)_repositories[typeof(TAggregate)];
		}

		public Task Commit()
		{
			throw new NotImplementedException();
		}

		public Task Abort()
		{
			throw new NotImplementedException();
		}
	}
}
