using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	class PipelineLookup : IPipelineLookup
	{
		private readonly Dictionary<string, IEnumerable<IPipeline>> _dictionary = new Dictionary<string, IEnumerable<IPipeline>>();

		public void Add(string key, IPipeline pipeline)
		{
			Add(key, new[] { pipeline });
		}

		public void Add(string key, IEnumerable<IPipeline> pipelines)
		{
			_dictionary.Add(key, pipelines);
		}

		public bool TryGet(string key, out IPipeline pipeline)
		{
			IEnumerable<IPipeline> pipelines;
			pipeline = _dictionary.TryGetValue(key, out pipelines)
				           ? pipelines.Single()
				           : null;
			return pipeline != null;
		}

		public bool TryGet(string key, out IEnumerable<IPipeline> pipelines)
		{
			return _dictionary.TryGetValue(key, out pipelines);
		}
	}
}