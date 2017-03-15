﻿using System;
using Maestro.Lifetimes;

namespace Maestro.Configuration
{
	public class DefaultLifetimeSelector
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
			Use<ScopedLifetime>();
		}

		public void Singleton()
		{
			Use<SingletonLifetime>();
		}

		public void Use<TLifetime>() where TLifetime : ILifetime, new()
		{
			Use(() => new TLifetime());
		}

		public void Use(Func<ILifetime> factory)
		{
			_action(factory);
		}
	}
}