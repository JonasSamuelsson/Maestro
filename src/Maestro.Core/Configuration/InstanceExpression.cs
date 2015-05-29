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
			return Execute(new ActionInterceptor<TInstance>(action));
		}

		public IInstanceExpression<TInstance, TParent> Execute(Func<TInstance, TInstance> func)
		{
			return Execute((instance, ctx) => func(instance));
		}

		public IInstanceExpression<TInstance, TParent> Execute(Func<TInstance, IContext, TInstance> func)
		{
			return Execute(new FuncInterceptor<TInstance>(func));
		}

		public IInstanceExpression<TInstance, TParent> Execute(IInterceptor interceptor)
		{
			Plugin.Interceptors.Add(interceptor);
			return this;
		}
	}
}