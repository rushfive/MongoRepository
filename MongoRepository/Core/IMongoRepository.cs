using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public interface IMongoRepository<TAggregate, TId> : IAggregateOperationStore, ISessionIdentityCache
		where TAggregate : class
	{
		Task<TAggregate> FindOne(TId id);
		Task<TAggregate> FindOneWhere(Expression<Func<TAggregate, bool>> predicate);
		Task<IReadOnlyCollection<TAggregate>> Query(Expression<Func<TAggregate, bool>> predicate);
		void Add(TAggregate aggregate);
		void Delete(TAggregate aggregate);
	}
}
