using System;
using System.Collections.Concurrent;

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
			public Key(Type type, string name)
			{
				Type = type;
				Name = name;
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
				unchecked
				{
					return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				}
			}
		}
	}
}