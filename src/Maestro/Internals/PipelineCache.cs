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
			private readonly int _hashCode;

			public Key(Type type, string name)
			{
				_hashCode = CalculateHashCode(type, name);
				Type = type;
				Name = name;
			}

			private static int CalculateHashCode(Type type, string name)
			{
				return (type.GetHashCode() * 397) ^ name.GetHashCode();
			}

			public Type Type { get; }
			public string Name { get; }

			public bool Equals(Key other)
			{
				return Type == other.Type && string.Equals(Name, other.Name);
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