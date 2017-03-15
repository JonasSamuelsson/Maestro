using System;
using Maestro.Lifetimes;

namespace Maestro
{
	internal class DefaultSettings
	{
		public Func<ILifetime> LifetimeFactory { get; set; } = () => TransientLifetime.Instance;
	}
}