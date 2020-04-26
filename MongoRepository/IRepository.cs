using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using MongoDB.Driver;
using R5.MongoRepository.Core;

namespace R5.MongoRepository
{
	public interface IRepository<TAggregate, TId> : IAggregateOperationStore
		where TAggregate : IAggregateRoot<TId>
	{
		Task<Option<TAggregate>> FindOrNone(TId id);
		void Add(TAggregate aggregate);
		void Delete(TAggregate aggregate);
	}

	public interface IAggregateOperationStore
	{
		List<ICommitAggregateOperation> GetCommitOperations();
	}

	public interface ICommitAggregateOperation
	{
		Task Execute(IMongoSessionContext sessionContext);
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

	//public interface IAggregateIdentityMap<TAggregate, TId>
	//	where TAggregate : IAggregateRoot<TId>
	//{
	//	bool ContainsEntry(TId id);
	//	TAggregate Get(TId id);
	//	bool TryGet(TId id, out TAggregate aggregate);
	//	void Set(TAggregate aggregate);
	//	void Delete(TAggregate aggregate);
	//	List<AggregateIdentityMap<TAggregate, TId>.Entry> GetEntries();
	//}

	public sealed class AggregateIdentityMap<TAggregate, TId>// : IAggregateIdentityMap<TAggregate, TId>
		where TAggregate : class, IAggregateRoot<TId>
	{
		private readonly Dictionary<TId, Entry> _entries = new Dictionary<TId, Entry>();

		public bool ContainsEntry(TId id)
		{
			return _entries.ContainsKey(id);
		}

		public TAggregate Get(TId id)
		{
			if (_entries.TryGetValue(id, out Entry entry) 
				&& entry.State != EntryState.Deleted)
			{
				return entry.Aggregate;
			}

			return null;
		}

		public bool TryGet(TId id, out TAggregate aggregate)
		{
			if (_entries.TryGetValue(id, out Entry entry)
				&& entry.State != EntryState.Deleted)
			{
				aggregate = entry.Aggregate;
				return true;
			}

			aggregate = null;
			return false;
		}

		public void Set(TAggregate aggregate)
		{
			if (IsMarkedAsDeleted(aggregate))
			{
				throw new InvalidOperationException($"Can't set aggregate '{aggregate.Id}' because it's been marked as deleted.");
			}

			_entries[aggregate.Id] = new Entry(aggregate);
		}

		private bool IsMarkedAsDeleted(TAggregate aggregate)
		{
			return _entries.TryGetValue(aggregate.Id, out Entry entry) && entry.State == EntryState.Deleted;
		}

		public void Delete(TAggregate aggregate)
		{
			if (!_entries.TryGetValue(aggregate.Id, out Entry entry))
			{
				throw new ArgumentException($"Entry for aggregate '{aggregate.Id}' doesn't exist in identity map.", nameof(aggregate.Id));
			}
			entry.MarkAsDeleted();
		}

		public List<Entry> GetEntries()
		{
			return _entries.Values.ToList();
		}

		public class Entry
		{
			public readonly TAggregate Aggregate;
			public EntryState State { get; private set; }

			internal Entry(TAggregate aggregate)
			{
				Aggregate = aggregate;
			}

			internal void MarkAsDeleted()
			{
				State = EntryState.Deleted;
			}
		}
	}

	public enum EntryState
	{
		// Exists in the session.
		// Either hydrated from the db or added as a new aggregate in memory
		Loaded,

		// Marked as deleted in the session but still exists in db
		Deleted
	}

	internal sealed class AggregateSaveOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		private readonly TAggregate _aggregate;
		private readonly Func<TAggregate, TDocument> _toDocument;

		internal AggregateSaveOperation(
			TAggregate aggregate,
			Func<TAggregate, TDocument> toDocument)
		{
			_aggregate = aggregate;
			_toDocument = toDocument;
		}

		public Task Execute(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			return sessionContext.GetCollection<TDocument>()
				.ReplaceOneAsync(
					Builders<TDocument>.Filter.Eq(d => d.Id, _aggregate.Id), 
					document, 
					new UpdateOptions { IsUpsert = true });
		}
	}

	public sealed class AggregateDeleteOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		private readonly TAggregate _aggregate;

		internal AggregateDeleteOperation(TAggregate aggregate)
		{
			_aggregate = aggregate;
		}

		public Task Execute(IMongoSessionContext sessionContext)
		{
			return sessionContext.GetCollection<TDocument>()
				.DeleteOneAsync(Builders<TDocument>.Filter.Eq(d => d.Id, _aggregate.Id));
		}
	}
}
