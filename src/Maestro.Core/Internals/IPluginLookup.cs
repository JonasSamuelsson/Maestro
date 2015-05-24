using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	interface IPluginLookup
	{
		bool TryGet(Type type, string name, out Plugin plugin);
		IEnumerable<Plugin> GetAll(Type type);
	}
}