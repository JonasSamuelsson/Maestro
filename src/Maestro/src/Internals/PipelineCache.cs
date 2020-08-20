using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class PipelineCache
	{
		private readonly ConcurrentDictionary<Key, Pipeline> _dictionary = new ConcurrentDictionary<Key, Pipeline>();

		public void Add(Key key, Pipeline pipeline)
		{
			if (_dictionary.TryAdd(key, pipeline)) return;
			throw new InvalidOperationException();
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool TryGet(Key key, out Pipeline pipeline)
		{
			return _dictionary.TryGetValue(key, out pipeline);
		}

		internal struct Key
		{
			private static readonly ConcurrentDictionary<Type, object> DefaultServices = new ConcurrentDictionary<Type, object>();
			private static readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, object>> NamedServices = new ConcurrentDictionary<string, ConcurrentDictionary<Type, object>>();

			private readonly int _hashCode;

			public Key(Type type, string name)
			{
				_hashCode = CalculateHashCode(name, type);
				Name = name;
				Type = type;
			}

			public Type Type { get; }
			public string Name { get; }

			public bool Equals(Key other)
			{
				return _hashCode == other._hashCode;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				return obj is Key key && Equals(key);
			}

			public override int GetHashCode()
			{
				return _hashCode;
			}

			private static int CalculateHashCode(string name, Type type)
			{
				if (name == ServiceNames.Default)
				{
					return GetOrCalculateHashCode(type, DefaultServices);
				}

				if (!NamedServices.TryGetValue(name, out var innerDictionary))
				{
					lock (NamedServices)
					{
						innerDictionary = NamedServices.GetOrAdd(name, _ => new ConcurrentDictionary<Type, object>());
					}
				}

				return GetOrCalculateHashCode(type, innerDictionary);
			}

			private static int GetOrCalculateHashCode(Type type, ConcurrentDictionary<Type, object> dictionary)
			{
				if (!dictionary.TryGetValue(type, out var o))
				{
					o = dictionary[type] = new object();
				}

				return o.GetHashCode();
			}
		}

		public void Populate(List<Diagnostics.Pipeline> pipelines)
		{
			_dictionary
				.ToArray()
				.ForEach(kvp =>
				{
					var pipeline = new Diagnostics.Pipeline
					{
						Type = kvp.Key.Type,
						Name = kvp.Key.Name
					};
					kvp.Value.Populate(pipeline.Services);
					pipelines.Add(pipeline);
				});
		}
	}
}