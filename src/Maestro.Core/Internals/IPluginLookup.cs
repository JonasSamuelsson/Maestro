using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	interface IPluginLookup
	{
		bool TryGet(Type type, string name, out IPlugin plugin);
		IEnumerable<IPlugin> GetAll(Type type);
	}
}