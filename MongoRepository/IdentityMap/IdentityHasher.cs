using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R5.MongoRepository.IdentityMap
{
	internal sealed class AggregateIdentityHasher
	{
		private readonly List<PropertyInfo> _properties;
		private readonly List<FieldInfo> _fields;

		private AggregateIdentityHasher(
			List<PropertyInfo> properties,
			List<FieldInfo> fields)
		{
			_properties = properties;
			_fields = fields;
		}

		internal int ComputeFor(object aggregate)
		{
			unchecked   //allow overflow
			{
				int hash = 17;
				foreach (var prop in _properties)
				{
					var value = prop.GetValue(aggregate, null);
					hash = HashValue(hash, value);
				}

				foreach (var field in _fields)
				{
					var value = field.GetValue(aggregate);
					hash = HashValue(hash, value);
				}

				return hash;
			}
		}

		private int HashValue(int seed, object value)
		{
			var currentHash = value?.GetHashCode() ?? 0;
			return seed * 23 + currentHash;
		}

		internal static AggregateIdentityHasher ResolveFor(object aggregate)
		{
			var properties = aggregate.GetType()
				.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.ToList();

			var fields = aggregate.GetType()
				.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.ToList();

			return new AggregateIdentityHasher(properties, fields);
		}
	}
}
