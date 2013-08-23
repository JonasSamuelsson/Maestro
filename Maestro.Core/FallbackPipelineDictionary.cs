using System;
using System.Collections.Generic;

namespace Maestro
{
	internal class FallbackPipelineDictionary : ICustomDictionary<IPipelineEngine>
	{
		private readonly Dictionary<Type, IPipelineEngine> _dictionary = new Dictionary<Type, IPipelineEngine>();

		public IPipelineEngine GetOrAdd(Type type)
		{
			IPipelineEngine pipelineEngine;
			if (!_dictionary.TryGetValue(type, out pipelineEngine))
				lock (_dictionary)
					if (!_dictionary.TryGetValue(type, out pipelineEngine))
					{
						pipelineEngine = new PipelineEngine(new TypeInstanceProvider(type));
						_dictionary.Add(type, pipelineEngine);
					}
			return pipelineEngine;
		}

		public bool TryGet(Type type, out IPipelineEngine pipelineEngine)
		{
			return _dictionary.TryGetValue(type, out pipelineEngine);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public bool Contains(Type type)
		{
			return _dictionary.ContainsKey(type);
		}
	}
}