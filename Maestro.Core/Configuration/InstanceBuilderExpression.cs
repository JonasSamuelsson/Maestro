using System;
using System.Linq.Expressions;
using Maestro.Interceptors;
using Maestro.Utils;

namespace Maestro.Configuration
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

		public IInstanceBuilderExpression<TInstance> Execute(Action<TInstance> action)
		{
			return Execute((instance, context) => action(instance));
		}

		public IInstanceBuilderExpression<TInstance> Execute(Action<TInstance, IContext> action)
		{
			return Intercept(new ActionInterceptor<TInstance>(action));
		}

		public IInstanceBuilderExpression<TInstance> Intercept(IInterceptor interceptor)
		{
			_instanceBuilder.AddInterceptor(interceptor);
			return this;
		}

		public IInstanceBuilderExpression<TOut> Intercept<TOut>(IInterceptor<TInstance, TOut> interceptor)
		{
			_instanceBuilder.AddInterceptor(interceptor);
			return new InstanceBuilderExpression<TOut>(_instanceBuilder);
		}

		public IInstanceBuilderExpression<TOut> Intercept<TOut>(Func<TInstance, TOut> lambda)
		{
			return Intercept((instance, context) => lambda(instance));
		}

		public IInstanceBuilderExpression<TOut> Intercept<TOut>(Func<TInstance, IContext, TOut> lambda)
		{
			return Intercept(new LambdaInterceptor<TInstance, TOut>(lambda));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property)
		{
			return Intercept(new SetPropertyInterceptor(property));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property, object value)
		{
			return Intercept(new SetPropertyInterceptor(property, _ => value));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property, Func<object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, _ => factory()));
		}

		public IInstanceBuilderExpression<TInstance> Set(string property, Func<IContext, object> factory)
		{
			return Intercept(new SetPropertyInterceptor(property, factory));
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return Set(property.GetName());
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value)
		{
			var propertyName = property.GetName();
			return Intercept(new SetPropertyInterceptor(propertyName, _ => value));
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			var propertyName = property.GetName();
			return Intercept(new SetPropertyInterceptor(propertyName, _ => factory()));
		}

		public IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			var propertyName = property.GetName();
			return Intercept(new SetPropertyInterceptor(propertyName, ctx => factory(ctx)));
		}

		public IInstanceBuilderExpression<TInstance> TrySet(string property)
		{
			return Intercept(new TrySetPropertyInterceptor(property));
		}

		public IInstanceBuilderExpression<TInstance> TrySet<TValue>(Expression<Func<TInstance, TValue>> property)
		{
			return TrySet(property.GetName());
		}
	}
}