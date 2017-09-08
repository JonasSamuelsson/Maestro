using Maestro.Utils;

namespace Maestro.Internals
{
	class PipelineCache<TKey>
	{
		private readonly ThreadSafeDictionary<TKey, IPipeline> _dictionary = new ThreadSafeDictionary<TKey, IPipeline>();

		public void Add(TKey key, IPipeline pipeline)
		{
			_dictionary.AddOrUpdate(key, pipeline);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool TryGet(TKey key, out IPipeline pipeline)
		{
			return _dictionary.TryGet(key, out pipeline);
		}
	}
}