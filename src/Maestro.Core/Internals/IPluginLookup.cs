using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	interface IPluginLookup
	{
		bool TryGet(Type type, string name, out ServiceDescriptor serviceDescriptor);
		IEnumerable<ServiceDescriptor> GetAll(Type type);
	}
}