using System.Collections.Generic;
using Maestro.Utils;

namespace Maestro
{
	internal class Plugin
	{
		private readonly ThreadSafeDictionary<string, IInstanceBuilder> _dictionary = new ThreadSafeDictionary<string, IInstanceBuilder>();

		public void Add(string name, IInstanceBuilder instanceBuilder)
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

		public IEnumerable<string> GetNames()
		{
			return _dictionary.Keys;
		}
	}
}