using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class Plugin : IEnumerable<KeyValuePair<string, IInstanceBuilder>>
	{
		public static readonly Plugin Empty = new EmptyPlugin();
        private readonly ConcurrentDictionary<string, IInstanceBuilder> _dictionary;

		public Plugin() : this(Enumerable.Empty<KeyValuePair<string, IInstanceBuilder>>()) { }

		public Plugin(IEnumerable<KeyValuePair<string, IInstanceBuilder>> keyValuePairs)
		{
            _dictionary = new ConcurrentDictionary<string, IInstanceBuilder>(keyValuePairs);
		}

		public virtual void Add(string name, IInstanceBuilder instanceBuilder)
		{
			_dictionary.AddOrUpdate(name, instanceBuilder, (n, ib) => ib);
		}

		public IInstanceBuilder Get(string name)
		{
		    IInstanceBuilder instanceBuilder;
            return TryGet(name, out instanceBuilder) ? instanceBuilder : null;
		}

		public bool TryGet(string name, out IInstanceBuilder instanceBuilder)
		{
			return _dictionary.TryGetValue(name, out instanceBuilder);
		}

		public Plugin Clone()
		{
			var plugin = new Plugin();
			foreach (var pair in _dictionary)
				plugin.Add(pair.Key, pair.Value.Clone());
			return plugin;
		}

		public IEnumerator<KeyValuePair<string, IInstanceBuilder>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class EmptyPlugin : Plugin
		{
			public override void Add(string name, IInstanceBuilder instanceBuilder)
			{
				throw new NotSupportedException();
			}
		}
	}
}