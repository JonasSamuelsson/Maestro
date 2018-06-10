using Maestro.Lifetimes;
using System;

namespace Maestro.Configuration
{
	internal class DefaultLifetimeSelector : IDefaultLifetimeSelector
	{
		private readonly Action<Func<ILifetime>> _action;

		internal DefaultLifetimeSelector(Action<Func<ILifetime>> action)
		{
			_action = action;
		}

		public void Transient()
		{
			Use(() => TransientLifetime.Instance);
		}

		public void Scoped()
		{
			Use(new ScopedLifetime());
		}

		public void Singleton()
		{
			Use(new SingletonLifetime());
		}

		private void Use(Func<ILifetime> factory)
		{
			_action(factory);
		}
	}
}