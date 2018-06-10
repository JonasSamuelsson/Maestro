using Maestro.Configuration;
using Maestro.Lifetimes;
using System;

namespace Maestro
{
	internal class ContainerSettings
	{
		internal Func<Lifetime> LifetimeFactory { get; set; } = () => TransientLifetime.Instance;
		internal GetServicesOrder GetServicesOrder { get; set; } = GetServicesOrder.Undefined;
	}
}