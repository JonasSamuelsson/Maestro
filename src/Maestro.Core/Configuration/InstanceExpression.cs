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

		public IInstanceExpression<TInstance, TParent> Intercept(Action<TInstance> action)
		{
			return Intercept((instance, ctx) => action(instance));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(Action<TInstance, IContext> action)
		{
			return Intercept(new ActionInterceptor<TInstance>(action));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(Func<TInstance, TInstance> func)
		{
			return Intercept((instance, ctx) => func(instance));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(Func<TInstance, IContext, TInstance> func)
		{
			return Intercept(new FuncInterceptor<TInstance>(func));
		}

		public IInstanceExpression<TInstance, TParent> Intercept(IInterceptor interceptor)
		{
			Plugin.Interceptors.Add(interceptor);
			return this;
		}
	}
}