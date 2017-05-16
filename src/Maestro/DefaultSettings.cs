using System;
using Maestro.Configuration;
using Maestro.Lifetimes;

namespace Maestro
{
	internal class DefaultSettings
	{
		public Func<ILifetime> LifetimeFactory { get; set; } = () => TransientLifetime.Instance;
		public GetServicesOrder GetServicesOrder { get; set; } = GetServicesOrder.Undefined;
	}
}