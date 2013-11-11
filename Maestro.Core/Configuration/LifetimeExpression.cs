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
			return Custom(TransientLifetime.Instance);
		}

		public TParent Request()
		{
			return Custom<RequestLifetime>();
		}

		public TParent Singleton()
		{
			return Custom<SingletonLifetime>();
		}

		public TParent Custom<TLifetime>() where TLifetime : ILifetime, new()
		{
			return Custom(new TLifetime());
		}

		public TParent Custom(ILifetime lifetime)
		{
			_action(lifetime);
			return _parent;
		}
	}
}