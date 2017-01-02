using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class ServiceFamily
	{
		public Type Type { get; set; }
		public Dictionary<string, ServiceDescriptor> Services { get; } = new Dictionary<string, ServiceDescriptor>();
		public List<ServiceDescriptor> Services_Obsolete { get; } = new List<ServiceDescriptor>();
	}
}