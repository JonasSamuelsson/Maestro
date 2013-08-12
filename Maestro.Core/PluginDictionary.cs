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

		public bool TryGet(Type type, out IPlugin plugin)
		{
			return _dictionary.TryGetValue(type, out plugin);
		}

		public void Clear()
		{
			_dictionary.Clear();
		}
	}
}