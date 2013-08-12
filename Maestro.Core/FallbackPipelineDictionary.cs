using System;
using System.Collections.Generic;

namespace Maestro
{
	internal class FallbackPipelineDictionary : ICustomDictionary<IPipeline>
	{
		private readonly Dictionary<Type, IPipeline> _dictionary = new Dictionary<Type, IPipeline>();

		public IPipeline GetOrAdd(Type type)
		{
			IPipeline pipeline;
			if (!_dictionary.TryGetValue(type, out pipeline))
				lock (_dictionary)
					if (!_dictionary.TryGetValue(type, out pipeline))
					{
						pipeline = new Pipeline(new TypeInstanceProvider(type));
						_dictionary.Add(type, pipeline);
					}
			return pipeline;
		}

		public bool TryGet(Type type, out IPipeline pipeline)
		{
			return _dictionary.TryGetValue(type, out pipeline);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}
	}
}