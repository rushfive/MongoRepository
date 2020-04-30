using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.IdentityMap
{
	internal static class AggregateIdentityHasherCache
	{
		private static readonly Dictionary<Type, AggregateIdentityHasher> _hashersByType
			= new Dictionary<Type, AggregateIdentityHasher>();

		internal static AggregateIdentityHasher GetFor(object aggregate)
		{
			var type = aggregate.GetType();

			if (_hashersByType.TryGetValue(type, out AggregateIdentityHasher hasher))
			{
				return hasher;
			}

			_hashersByType[type] = AggregateIdentityHasher.ResolveFor(aggregate);
			return _hashersByType[type];
		}
	}
}
