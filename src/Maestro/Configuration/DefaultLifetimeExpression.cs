using Maestro.Lifetimes;
using System;

namespace Maestro.Configuration
{
	public class DefaultLifetimeExpression
	{
		private readonly ContainerSettings _containerSettings;

		internal DefaultLifetimeExpression(ContainerSettings containerSettings)
		{
			_containerSettings = containerSettings;
		}

		public void Transient()
		{
			Use(() => TransientLifetime.Instance);
		}

		public void Scoped()
		{
			Use(() => new ScopedLifetime());
		}

		public void Singleton()
		{
			Use(() => new SingletonLifetime());
		}

		private void Use(Func<Lifetime> factory)
		{
			_containerSettings.LifetimeFactory = factory;
		}
	}
}