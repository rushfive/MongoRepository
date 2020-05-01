using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.IdentityMap
{
	public class Entry<TAggregate>
		where TAggregate : class
	{
		public readonly TAggregate Aggregate;
		public EntryState State { get; private set; }

		private Entry(TAggregate aggregate, EntryState state)
		{
			Aggregate = aggregate;
			State = state;
		}

		internal static Entry<TAggregate> LoadedFromDatabase(TAggregate aggregate)
			=> new Entry<TAggregate>(aggregate, EntryState.Loaded);

		internal static Entry<TAggregate> AddedInMemory(TAggregate aggregate)
			=> new Entry<TAggregate>(aggregate, EntryState.Added);

		internal void MarkAsDeleted()
		{
			State = EntryState.Deleted;
		}
	}
}
