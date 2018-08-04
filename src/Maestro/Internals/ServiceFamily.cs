using Maestro.Utils;
using System;

namespace Maestro.Internals
{
	internal class ServiceFamily
	{
		public Type Type { get; set; }
		public ThreadSafeDictionary<string, ServiceDescriptor> Dictionary { get; } = new ThreadSafeDictionary<string, ServiceDescriptor>();
		public ThreadSafeList<ServiceDescriptor> List { get; } = new ThreadSafeList<ServiceDescriptor>();
	}
}