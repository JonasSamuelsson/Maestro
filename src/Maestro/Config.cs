using Maestro.Configuration;
using Maestro.Lifetimes;
using System;

namespace Maestro
{
	internal class Config
	{
		public Func<Lifetime> LifetimeFactory { get; set; } = () => TransientLifetime.Instance;
		public GetServicesOrder GetServicesOrder { get; set; } = GetServicesOrder.Undefined;
	}
}