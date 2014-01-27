using System;
using Maestro.Lifetimes;

namespace Maestro.Configuration
{
	internal class LifetimeExpression<TParent> : ILifetimeExpression<TParent>
	{
		private readonly TParent _parent;
		private readonly Action<ILifetime> _action;

		public LifetimeExpression(TParent parent, Action<ILifetime> action)
		{
			_parent = parent;
			_action = action;
		}

		public TParent Transient()
		{
			return Use(TransientLifetime.Instance);
		}

		public TParent Context()
		{
			return Use<ContextSingletonLifetime>();
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
			_action(lifetime);
			return _parent;
		}
	}
}