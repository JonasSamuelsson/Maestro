using System;

namespace Maestro.Diagnostics
{
	internal class Service
	{
		public Type InstanceType { get; internal set; }
		public string Lifetime { get; internal set; }
		public string Name { get; internal set; }
		public string Provider { get; internal set; }
		public Type ServiceType { get; internal set; }
	}
}