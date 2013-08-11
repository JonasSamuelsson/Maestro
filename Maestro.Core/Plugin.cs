using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class Plugin : IPlugin
	{
		private readonly Dictionary<string, IPipeline> _dictionary = new Dictionary<string, IPipeline>();

		public void Add(string name, IPipeline pipeline)
		{
			_dictionary.Add(name, pipeline);
		}

		public IPipeline Get(string name)
		{
			return _dictionary[name];
		}

		public bool TryGet(string name, out IPipeline pipeline)
		{
			return _dictionary.TryGetValue(name, out pipeline);
		}

		public IEnumerable<string> GetNames()
		{
			return _dictionary.Keys;
		}
	}
}