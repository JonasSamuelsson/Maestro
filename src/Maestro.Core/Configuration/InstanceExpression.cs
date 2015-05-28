using System;
using Maestro.Interceptors;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class InstanceExpression<TInstance, TParent> : IInstanceExpression<TInstance, TParent>
	{
		public InstanceExpression(Plugin plugin, TParent parent)
		{
			Plugin = plugin;
			Parent = parent;
		}

		internal Plugin Plugin { get; set; }
		internal TParent Parent { get; set; }

		public ILifetimeExpression<TParent> Lifetime
		{
			get { return new LifetimeExpression<TParent>(Parent, lifetime => Plugin.Lifetime = lifetime); }
		}

		public IInstanceExpression<TInstance, TParent> Execute(Action<TInstance> action)
		{
			return Execute((instance, ctx) => action(instance));
		}

		public IInstanceExpression<TInstance, TParent> Execute(Action<TInstance, IContext> action)
		{
			return Intercept(new ExecuteActionInterceptor<TInstance>(action));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(IInterceptor interceptor)
		{
			Plugin.Interceptors.Add(interceptor);
			return this;
		}
	}
}