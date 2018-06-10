using Maestro.Lifetimes;
using System;

namespace Maestro.Configuration
{
	internal class LifetimeSelector<TParent> : ILifetimeSelector<TParent>
	{
		private readonly TParent _parent;
		private readonly Action<Func<Lifetime>> _action;

		internal LifetimeSelector(TParent parent, Action<Func<Lifetime>> action)
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

		private TParent Use(Lifetime lifetime)
		{
			_action(() => lifetime);
			return _parent;
		}
	}
}