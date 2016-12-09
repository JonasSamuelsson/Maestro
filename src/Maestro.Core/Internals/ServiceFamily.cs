using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class ServiceFamily
	{
		public Type Type { get; set; }
		public Dictionary<string, ServiceDescriptor> Service { get; } = new Dictionary<string, ServiceDescriptor>();
		public List<ServiceDescriptor> Services { get; } = new List<ServiceDescriptor>();
	}
}