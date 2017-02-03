using System.Collections.Generic;

namespace Maestro.Internals
{
	class PipelineCache<TKey>
	{
		private readonly Dictionary<TKey, IPipeline> _dictionary = new Dictionary<TKey, IPipeline>();

		public void Add(TKey key, IPipeline pipeline)
		{
			_dictionary.Add(key, pipeline);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool TryGet(TKey key, out IPipeline pipeline)
		{
			return _dictionary.TryGetValue(key, out pipeline);
		}
	}
}