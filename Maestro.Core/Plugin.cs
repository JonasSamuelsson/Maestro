using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maestro.Utils;

namespace Maestro
{
	internal class Plugin : IEnumerable<KeyValuePair<string, IInstanceBuilder>>
	{
		public static readonly Plugin Empty = new EmptyPlugin();
		private readonly ThreadSafeDictionary<string, IInstanceBuilder> _dictionary;

		public Plugin() : this(Enumerable.Empty<KeyValuePair<string, IInstanceBuilder>>()) { }

		public Plugin(IEnumerable<KeyValuePair<string, IInstanceBuilder>> keyValuePairs)
		{
			_dictionary = new ThreadSafeDictionary<string, IInstanceBuilder>(keyValuePairs);
		}

		public virtual void Add(string name, IInstanceBuilder instanceBuilder)
		{
			_dictionary.Add(name, instanceBuilder);
		}

		public IInstanceBuilder Get(string name)
		{
			return _dictionary.Get(name);
		}

		public bool TryGet(string name, out IInstanceBuilder instanceBuilder)
		{
			return _dictionary.TryGet(name, out instanceBuilder);
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