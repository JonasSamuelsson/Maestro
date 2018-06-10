using Maestro.Lifetimes;
using System;

namespace Maestro.Configuration
{
	internal class DefaultLifetimeSelector : IDefaultLifetimeSelector
	{
		private readonly Action<Func<Lifetime>> _action;

		internal DefaultLifetimeSelector(Action<Func<Lifetime>> action)
		{
			_action = action;
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
			_action(factory);
		}
	}
}