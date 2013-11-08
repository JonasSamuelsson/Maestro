using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Fluent
{
	internal class InstanceBuilderExpression<TInstance> : IInstanceBuilderExpression<TInstance>
	{
		private readonly IInstanceBuilder _instanceBuilder;

		public InstanceBuilderExpression(IInstanceBuilder instanceBuilder)
		{
			_instanceBuilder = instanceBuilder;
		}

		public ILifetimeExpression<IInstanceBuilderExpression<TInstance>> Lifetime
		{
			get { return new LifetimeExpression<IInstanceBuilderExpression<TInstance>>(this, _instanceBuilder.SetLifetime); }
		}

		public IInstanceBuilderExpression<TInstance> AsSingleton()
		{
			return Lifetime.Singleton();
		}

		public IInstanceBuilderExpression<TInstance> AsTransient()
		{
			return Lifetime.Transient();
		}

		public IInstanceBuilderExpression<TInstance> Execute(Action<TInstance> action)
		{
			return Execute((instance, context) => action(instance));
		}

		public IInstanceBuilderExpression<TInstance> Execute(Action<TInstance, IContext> action)
		{
			return InterceptWith(new ActionInterceptor<TInstance>(action));
		}

		public IInstanceBuilderExpression<TInstance> InterceptWith(IInterceptor interceptor)
		{
			_instanceBuilder.AddInterceptor(interceptor);
			return this;
		}

		public IInstanceBuilderExpression<TOut> InterceptWith<TOut>(IInterceptor<TInstance, TOut> interceptor)
		{
			_instanceBuilder.AddInterceptor(interceptor);
			return new InstanceBuilderExpression<TOut>(_instanceBuilder);
		}

		public IInstanceBuilderExpression<TOut> InterceptWith<TOut>(Func<TInstance, TOut> lambda)
		{
			return InterceptWith((instance, context) => lambda(instance));
		}

		public IInstanceBuilderExpression<TOut> InterceptWith<TOut>(Func<TInstance, IContext, TOut> lambda)
		{
			return InterceptWith(new LambdaInterceptor<TInstance, TOut>(lambda));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property)
		{
			return InterceptWith(new SetPropertyInterceptor(property));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property, object value)
		{
			return InterceptWith(new SetPropertyInterceptor(property, _ => value));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property, Func<object> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property, _ => factory()));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property, Func<IContext, object> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property, factory));
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return Set(property.GetName());
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value)
		{
			var propertyName = property.GetName();
			return InterceptWith(new SetPropertyInterceptor(propertyName, _ => value));
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			var propertyName = property.GetName();
			return InterceptWith(new SetPropertyInterceptor(propertyName, _ => factory()));
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			var propertyName = property.GetName();
			return InterceptWith(new SetPropertyInterceptor(propertyName, ctx => factory(ctx)));
		}

		public IInstanceBuilderExpression<TInstance> TrySet(string property)
		{
			return InterceptWith(new TrySetPropertyInterceptor(property));
		}

		public IInstanceBuilderExpression<TInstance> TrySet<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return TrySet(property.GetName());
		}
	}
}