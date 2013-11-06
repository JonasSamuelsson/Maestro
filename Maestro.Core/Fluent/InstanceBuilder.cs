using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Fluent
{
	internal class InstanceBuilder<TInstance> : IInstanceBuilder<TInstance>
	{
		private readonly IPipelineEngine _pipelineEngine;

		public InstanceBuilder(IPipelineEngine pipelineEngine)
		{
			_pipelineEngine = pipelineEngine;
		}

		public ILifetimeExpression<IInstanceBuilder<TInstance>> Lifetime
		{
			get { return new LifetimeExpression<IInstanceBuilder<TInstance>>(this, _pipelineEngine.SetLifetime); }
		}

		public IInstanceBuilder<TInstance> AsSingleton()
		{
			return Lifetime.Singleton();
		}

		public IInstanceBuilder<TInstance> AsTransient()
		{
			return Lifetime.Transient();
		}

		public IInstanceBuilder<TInstance> Execute(Action<TInstance> action)
		{
			return Execute((instance, context) => action(instance));
		}

		public IInstanceBuilder<TInstance> Execute(Action<TInstance, IContext> action)
		{
			return InterceptWith(new ActionInterceptor<TInstance>(action));
		}

		public IInstanceBuilder<TInstance> InterceptWith(IInterceptor interceptor)
		{
			_pipelineEngine.AddInterceptor(interceptor);
			return this;
		}

		public IInstanceBuilder<TOut> InterceptWith<TOut>(IInterceptor<TInstance, TOut> interceptor)
		{
			_pipelineEngine.AddInterceptor(interceptor);
			return new InstanceBuilder<TOut>(_pipelineEngine);
		}

		public IInstanceBuilder<TOut> InterceptWith<TOut>(Func<TInstance, TOut> lambda)
		{
			return InterceptWith((instance, context) => lambda(instance));
		}

		public IInstanceBuilder<TOut> InterceptWith<TOut>(Func<TInstance, IContext, TOut> lambda)
		{
			return InterceptWith(new LambdaInterceptor<TInstance, TOut>(lambda));
		}

		public IInstanceBuilder<TInstance> Set(string property)
		{
			return InterceptWith(new SetPropertyInterceptor(property));
		}

		public IInstanceBuilder<TInstance> Set(string property, object value)
		{
			return InterceptWith(new SetPropertyInterceptor(property, _ => value));
		}

		public IInstanceBuilder<TInstance> Set(string property, Func<object> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property, _ => factory()));
		}

		public IInstanceBuilder<TInstance> Set(string property, Func<IContext, object> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property, factory));
		}

		public IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return Set(property.GetName());
		}

		public IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value)
		{
			var propertyName = property.GetName();
			return InterceptWith(new SetPropertyInterceptor(propertyName, _ => value));
		}

		public IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			var propertyName = property.GetName();
			return InterceptWith(new SetPropertyInterceptor(propertyName, _ => factory()));
		}

		public IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			var propertyName = property.GetName();
			return InterceptWith(new SetPropertyInterceptor(propertyName, ctx => factory(ctx)));
		}

		public IInstanceBuilder<TInstance> TrySet(string property)
		{
			return InterceptWith(new TrySetPropertyInterceptor(property));
		}

		public IInstanceBuilder<TInstance> TrySet<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return TrySet(property.GetName());
		}
	}
}