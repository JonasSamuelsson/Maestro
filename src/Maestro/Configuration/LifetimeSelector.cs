using Maestro.Lifetimes;
using System;

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
			return Use(new ScopedLifetime());
		}

		public TParent Singleton()
		{
			return Use(new SingletonLifetime());
		}

		private TParent Use(ILifetime lifetime)
		{
			_action(() => lifetime);
			return _parent;
		}
	}
}