using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using MongoDB.Bson;
using MongoDB.Driver;

namespace R5.MongoRepository.Core
{
	public interface IRepository<TAggregate, TId> : IAggregateOperationStore, ISessionIdentityCache
		where TAggregate : IAggregateRoot<TId>
	{
		Task<TAggregate> FindOrDefault(TId id);
		void Add(TAggregate aggregate);
		void Delete(TAggregate aggregate);
	}

	

	public interface ISessionIdentityCache
	{
		void OnTransactionCommitOrAborted();
	}

	public interface ICommitAggregateOperation
	{
		Task ExecuteAsync(IMongoSessionContext sessionContext);
		void Execute(IMongoSessionContext sessionContext);
	}

	public interface IAggregateRoot<TId>
	{
		TId Id { get; }
	}

	public interface IAggregateDocument<TId>
	{
		TId Id { get; }
		string SessionLock { get; }
	}

	public interface IAggregateMapper<TAggregate, TDocument, TId>
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		TDocument ToDocument(TAggregate aggregate);
		TAggregate ToAggregate(TDocument document);
	}

	public sealed class AggregateIdentityMap<TAggregate, TId>// : IAggregateIdentityMap<TAggregate, TId>
		where TAggregate : class, IAggregateRoot<TId>
	{
		private readonly Dictionary<TId, Entry> _entries = new Dictionary<TId, Entry>();

		// caches the aggregate hash values when loaded. compared to the hash computed
		// before commits to determine the need for a replace op
		private readonly Dictionary<TId, int> _loadedEntryHashes = new Dictionary<TId, int>();

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

		public void Add(TAggregate aggregate)
		{
			_entries.Add(aggregate.Id, Entry.AddedInMemory(aggregate));
		}

		public void SetFromLoad(TAggregate aggregate)
		{
			_entries.Add(aggregate.Id, Entry.LoadedFromDatabase(aggregate));

			AggregateIdentityHasher hasher = AggregateIdentityHasherCache.GetFor(aggregate);
			_loadedEntryHashes[aggregate.Id] = hasher.ComputeFor(aggregate);
		}

		public void Delete(TAggregate aggregate)
		{
			if (!_entries.TryGetValue(aggregate.Id, out Entry entry))
			{
				throw new ArgumentException($"Entry for aggregate '{aggregate.Id}' doesn't exist in identity map.", nameof(aggregate.Id));
			}
			entry.MarkAsDeleted();
		}

		// not all entries need an operation executed..
		public List<Entry> GetCommitableEntries()
			=> _entries.Values
				.Where(RequiresCommitOperation)
				.ToList();

		private bool RequiresCommitOperation(Entry entry)
		{
			switch (entry.State)
			{
				case EntryState.Loaded:
					AggregateIdentityHasher hasher = AggregateIdentityHasherCache.GetFor(entry.Aggregate);
					int currentHash = hasher.ComputeFor(entry.Aggregate);
					int hashOnLoad = _loadedEntryHashes[entry.Aggregate.Id];
					return currentHash != hashOnLoad;
				case EntryState.Added:
					return true;
				case EntryState.Deleted:
					bool existsInDatabase = _loadedEntryHashes.ContainsKey(entry.Aggregate.Id);
					return existsInDatabase;
				default:
					throw new ArgumentException($"'{entry.State}' is an invalid entry state type.");
			}
		}


		public void Reset()
		{
			_entries.Clear();
			_loadedEntryHashes.Clear();
		}

		public class Entry
		{
			public readonly TAggregate Aggregate;
			public EntryState State { get; private set; }

			private Entry(TAggregate aggregate, EntryState state)
			{
				Aggregate = aggregate;
				State = state;
			}

			internal static Entry LoadedFromDatabase(TAggregate aggregate)
				=> new Entry(aggregate, EntryState.Loaded);

			internal static Entry AddedInMemory(TAggregate aggregate)
				=> new Entry(aggregate, EntryState.Added);

			internal void MarkAsDeleted()
			{
				State = EntryState.Deleted;
			}
		}
	}

	public enum EntryState
	{
		// Exists in the session, hydrated existing aggregate from db
		Loaded,

		// Exists in the session through an add (exists only in-memory until committed)
		Added,

		// Marked as deleted in the session but still exists in db
		Deleted
	}






	public interface IAggregateOperationStore
	{
		List<ICommitAggregateOperation> GetCommitOperations();
	}

	public sealed class AggregateReplaceOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		private readonly TAggregate _aggregate;
		private readonly Func<TAggregate, TDocument> _toDocument;

		public AggregateReplaceOperation(
			TAggregate aggregate,
			Func<TAggregate, TDocument> toDocument)
		{
			_aggregate = aggregate;
			_toDocument = toDocument;
		}

		public void Execute(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			sessionContext.GetCollection<TDocument>()
				.ReplaceOne(
					Builders<TDocument>.Filter.Eq(d => d.Id, _aggregate.Id),
					document);
		}

		public Task ExecuteAsync(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			return sessionContext.GetCollection<TDocument>()
				.ReplaceOneAsync(
					Builders<TDocument>.Filter.Eq(d => d.Id, _aggregate.Id),
					document);
		}
	}

	public sealed class AggregateInsertOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		private readonly TAggregate _aggregate;
		private readonly Func<TAggregate, TDocument> _toDocument;

		public AggregateInsertOperation(
			TAggregate aggregate,
			Func<TAggregate, TDocument> toDocument)
		{
			_aggregate = aggregate;
			_toDocument = toDocument;
		}

		public void Execute(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			sessionContext.GetCollection<TDocument>().InsertOne(document);
		}

		public Task ExecuteAsync(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			return sessionContext.GetCollection<TDocument>().InsertOneAsync(document);
		}
	}

	public sealed class AggregateDeleteOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		private readonly TAggregate _aggregate;

		public AggregateDeleteOperation(TAggregate aggregate)
		{
			_aggregate = aggregate;
		}

		public void Execute(IMongoSessionContext sessionContext)
		{
			sessionContext.GetCollection<TDocument>()
				.DeleteOneAsync(Builders<TDocument>.Filter.Eq(d => d.Id, _aggregate.Id));
		}

		public Task ExecuteAsync(IMongoSessionContext sessionContext)
		{
			return sessionContext.GetCollection<TDocument>()
				.DeleteOneAsync(Builders<TDocument>.Filter.Eq(d => d.Id, _aggregate.Id));
		}
	}
}
