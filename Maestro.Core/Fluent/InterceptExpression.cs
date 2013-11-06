using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Fluent
{
	internal class InterceptExpression<T> : IInterceptExpression<T>
	{
		public IInterceptExpression<T> Execute(Action<T> action)
		{
			return Execute((instance, context) => action(instance));
		}

		public IInterceptExpression<T> Execute(Action<T, IContext> action)
		{
			return InterceptWith(new ActionInterceptor<T>(action));
		}

		public IInterceptExpression<T> InterceptWith(IInterceptor interceptor)
		{
			throw new NotImplementedException();
		}

		public IInterceptExpression<TOut> InterceptWith<TOut>(IInterceptor<T, TOut> interceptor)
		{
			throw new NotImplementedException();
		}

		public IInterceptExpression<TOut> InterceptWith<TOut>(Func<T, TOut> lambda)
		{
			return InterceptWith((instance, context) => lambda(instance));
		}

		public IInterceptExpression<TOut> InterceptWith<TOut>(Func<T, IContext, TOut> lambda)
		{
			return InterceptWith(new LambdaInterceptor<T, TOut>(lambda));
		}

		public IInterceptExpression<T> Set(string property)
		{
			return InterceptWith(new SetPropertyInterceptor(property));
		}

		public IInterceptExpression<T> Set(string property, object value)
		{
			return InterceptWith(new SetPropertyInterceptor(property, _ => value));
		}

		public IInterceptExpression<T> Set(string property, Func<object> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property, _ => factory()));
		}

		public IInterceptExpression<T> Set(string property, Func<IContext, object> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property, factory));
		}

		public IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property)
		{
			return Set(property.GetName());
		}

		public IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property, TValue value)
		{
			return InterceptWith(new SetPropertyInterceptor(property.GetName(), _ => value));
		}

		public IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property, Func<TValue> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property.GetName(), _ => factory()));
		}

		public IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property, Func<IContext, TValue> factory)
		{
			return InterceptWith(new SetPropertyInterceptor(property.GetName(), context => factory(context)));
		}

		public IInterceptExpression<T> TrySet(string property)
		{
			return InterceptWith(new TrySetPropertyInterceptor(property));
		}

		public IInterceptExpression<T> TrySet<TValue>(Expression<Func<T, TValue>> property)
		{
			return InterceptWith(new TrySetPropertyInterceptor(property.GetName()));
		}
	}
}