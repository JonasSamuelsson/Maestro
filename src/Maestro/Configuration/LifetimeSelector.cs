using System;
using Maestro.Lifetimes;

namespace Maestro.Configuration
{
	internal class LifetimeSelector<TParent> : ILifetimeSelector<TParent>
	{
		private readonly TParent _parent;
		private readonly Action<Func<ILifetime>> _action;

		internal LifetimeSelector(TParent parent, Action<Func<ILifetime>> action)
		{
			_parent = parent;
			_action = action;
		}

		public TParent Transient()
		{
			return Use(TransientLifetime.Instance);
		}

		public TParent Scoped()
		{
			return Use<ScopedLifetime>();
		}

		public TParent Singleton()
		{
			return Use<SingletonLifetime>();
		}

		public TParent Use<TLifetime>() where TLifetime : ILifetime, new()
		{
			return Use(new TLifetime());
		}

		public TParent Use(ILifetime lifetime)
		{
			_action(() => lifetime);
			return _parent;
		}
	}
}