using System.Collections.Generic;
using System.Linq;

namespace Maestro.Internals
{
	class PipelineCache<TKey>
	{
		private readonly Dictionary<TKey, IEnumerable<IPipeline>> _dictionary = new Dictionary<TKey, IEnumerable<IPipeline>>();

		public void Add(TKey key, IPipeline pipeline)
		{
			Add(key, new[] { pipeline });
		}

		public void Add(TKey key, IEnumerable<IPipeline> pipelines)
		{
			_dictionary.Add(key, pipelines);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool TryGet(TKey key, out IPipeline pipeline)
		{
			IEnumerable<IPipeline> pipelines;
			pipeline = _dictionary.TryGetValue(key, out pipelines)
							  ? pipelines.Single()
							  : null;
			return pipeline != null;
		}

		public bool TryGet(TKey key, out IEnumerable<IPipeline> pipelines)
		{
			return _dictionary.TryGetValue(key, out pipelines);
		}
	}
}