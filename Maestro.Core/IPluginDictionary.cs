using System;

namespace Maestro
{
	internal interface IPluginDictionary
	{
		IPlugin GetOrAdd(Type type);
		bool TryGet(Type type, out IPlugin plugin);
	}
}