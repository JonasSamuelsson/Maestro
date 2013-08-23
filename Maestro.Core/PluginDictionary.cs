using System;
using System.Collections.Generic;

namespace Maestro
{
	internal class PluginDictionary : ICustomDictionary<IPlugin>
	{
		private readonly Dictionary<Type, IPlugin> _dictionary = new Dictionary<Type, IPlugin>();

		public IPlugin GetOrAdd(Type type)
		{
			IPlugin plugin;
			if (!_dictionary.TryGetValue(type, out plugin))
				lock (_dictionary)
					if (!_dictionary.TryGetValue(type, out plugin))
					{
						plugin = new Plugin();
						_dictionary.Add(type, plugin);
					}
			return plugin;
		}

		public bool TryGet(Type type, out IPlugin pipelineEngine)
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