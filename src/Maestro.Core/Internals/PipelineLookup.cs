using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	class PipelineLookup
	{
		private readonly Dictionary<string, IEnumerable<Pipeline>> _dictionary = new Dictionary<string, IEnumerable<Pipeline>>();

		public void Add(string key, Pipeline pipeline)
		{
			Add(key, new[] { pipeline });
		}

		public void Add(string key, IEnumerable<Pipeline> pipelines)
		{
			_dictionary.Add(key, pipelines);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool TryGet(string key, out Pipeline pipeline)
		{
			IEnumerable<Pipeline> pipelines;
			pipeline = _dictionary.TryGetValue(key, out pipelines)
							  ? pipelines.Single()
							  : null;
			return pipeline != null;
		}

		public bool TryGet(string key, out IEnumerable<Pipeline> pipelines)
		{
			return _dictionary.TryGetValue(key, out pipelines);
		}
	}
}