using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace R5.MongoRepository
{
	public interface IRepository<TAggregate, TId>
		where TAggregate : IAggregateRoot<TId>
	{
		Task<Option<TAggregate>> FindOrNone(TId id);
		Task Add(TAggregate aggregate);
		Task Delete(TAggregate aggregate);
	}

	public interface IAggregateOperationStore
	{
		List<ICommitAggregateSaveOperation> GetCommitOperations();
	}

	public interface ICommitAggregateSaveOperation
	{
		object Id { get; }
		object Aggregate { get; }
		Task Save();
	}

	public interface IAggregateRoot<TId>
	{
		TId Id { get; }
	}

	public interface IAggregateDocument<TId>
	{
		TId Id { get; }
	}

	public interface IAggregateMapper<TAggregate, TDocument, TId>
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		TDocument ToDocument(TAggregate aggregate);
		TAggregate ToAggregate(TDocument document);
	}

	public interface IAggregateIdentityMap<TAggregate, TId>
		where TAggregate : IAggregateRoot<TId>
	{
		bool TryGet(TId id, out TAggregate aggregate);
		void Set(TId id, TAggregate aggregate);
		List<TAggregate> GetAll();
	}
}
