using System.Collections.Generic;

namespace Maestro
{
	internal class Plugin : IPlugin
	{
		private readonly IThreadSafeDictionary<string, IPipelineEngine> _dictionary = new ThreadSafeDictionary<string, IPipelineEngine>();

		public void Add(string name, IPipelineEngine pipelineEngine)
		{
			_dictionary.Add(name, pipelineEngine);
		}

		public IPipelineEngine Get(string name)
		{
			return _dictionary.Get(name);
		}

		public bool TryGet(string name, out IPipelineEngine pipelineEngine)
		{
			return _dictionary.TryGet(name, out pipelineEngine);
		}

		public IEnumerable<string> GetNames()
		{
			return _dictionary.Keys;
		}
	}
}