using Maestro.Utils;
using System;

namespace Maestro.Internals
{
	internal class ServiceFamily
	{
		public Type Type { get; set; }
		public ThreadSafeDictionary<string, ServiceDescriptor> NamedServices { get; } = new ThreadSafeDictionary<string, ServiceDescriptor>();
		public ThreadSafeList<ServiceDescriptor> AnonymousServices { get; } = new ThreadSafeList<ServiceDescriptor>();
	}
}