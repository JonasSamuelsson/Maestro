using System.Collections.Generic;

namespace Maestro
{
	internal class Plugin : IPlugin
	{
		private readonly Dictionary<string, IPipelineEngine> _dictionary = new Dictionary<string, IPipelineEngine>();

		public void Add(string name, IPipelineEngine pipelineEngine)
		{
			_dictionary.Add(name, pipelineEngine);
		}

		public IPipelineEngine Get(string name)
		{
			return _dictionary[name];
		}

		public bool TryGet(string name, out IPipelineEngine pipelineEngine)
		{
			return _dictionary.TryGetValue(name, out pipelineEngine);
		}

		public IEnumerable<string> GetNames()
		{
			return _dictionary.Keys;
		}
	}
}