using System;

namespace Maestro.Diagnostics
{
	internal class Service
	{
		internal int? Id { get; set; }
		internal Type InstanceType { get; set; }
		internal string Lifetime { get; set; }
		internal string Name { get; set; }
		internal string Provider { get; set; }
		internal Type ServiceType { get; set; }
	}
}