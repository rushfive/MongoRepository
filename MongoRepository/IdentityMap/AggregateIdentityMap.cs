using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R5.MongoRepository.IdentityMap
{
	public sealed class AggregateIdentityMap<TAggregate, TId>
		where TAggregate : class
	{
		private readonly Func<TAggregate, TId> _idSelector;
		private readonly Dictionary<TId, Entry> _entries = new Dictionary<TId, Entry>();

		// caches the aggregate hash values when loaded. compared to the hash computed
		// before commits to determine the need for a replace op
		private readonly Dictionary<TId, int> _loadedEntryHashes = new Dictionary<TId, int>();

		public AggregateIdentityMap(Func<TAggregate, TId> idSelector)
		{
			_idSelector = idSelector;
		}

		public bool IsBeingTracked(TId id)
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
			_entries.Add(_idSelector(aggregate), Entry.AddedInMemory(aggregate));
		}

		public void SetFromLoad(TAggregate aggregate)
		{
			TId aggregateId = _idSelector(aggregate);

			_entries.Add(aggregateId, Entry.LoadedFromDatabase(aggregate));

			AggregateIdentityHasher hasher = AggregateIdentityHasherCache.GetFor(aggregate);
			_loadedEntryHashes[aggregateId] = hasher.ComputeFor(aggregate);
		}

		public void Delete(TAggregate aggregate)
		{
			TId aggregateId = _idSelector(aggregate);

			if (!_entries.TryGetValue(aggregateId, out Entry entry))
			{
				throw new ArgumentException($"Entry for aggregate '{aggregateId}' doesn't exist in identity map.", nameof(aggregateId));
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
			TId aggregateId = _idSelector(entry.Aggregate);

			switch (entry.State)
			{
				case EntryState.Loaded:
					AggregateIdentityHasher hasher = AggregateIdentityHasherCache.GetFor(entry.Aggregate);
					int currentHash = hasher.ComputeFor(entry.Aggregate);
					int hashOnLoad = _loadedEntryHashes[aggregateId];
					return currentHash != hashOnLoad;
				case EntryState.Added:
					return true;
				case EntryState.Deleted:
					bool existsInDatabase = _loadedEntryHashes.ContainsKey(aggregateId);
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
}
