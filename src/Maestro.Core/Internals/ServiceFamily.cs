using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class ServiceFamily
	{
		public Type Type { get; set; }
		public Dictionary<string, ServiceDescriptor> NamedServices { get; } = new Dictionary<string, ServiceDescriptor>();
		public List<ServiceDescriptor> AnonymousServices { get; } = new List<ServiceDescriptor>();
	}
}