using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Fluent
{
	public interface IInterceptExpression<T>
	{
		IInterceptExpression<T> Execute(Action<T> action);
		IInterceptExpression<T> Execute(Action<T, IContext> action);
		IInterceptExpression<T> InterceptWith(IInterceptor interceptor);
		IInterceptExpression<TOut> InterceptWith<TOut>(IInterceptor<T, TOut> interceptor);
		IInterceptExpression<TOut> InterceptWith<TOut>(Func<T, TOut> lambda);
		IInterceptExpression<TOut> InterceptWith<TOut>(Func<T, IContext, TOut> lambda);
		IInterceptExpression<T> Set(string property);
		IInterceptExpression<T> Set(string property, object value);
		IInterceptExpression<T> Set(string property, Func<object> factory);
		IInterceptExpression<T> Set(string property, Func<IContext, object> factory);
		IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property);
		IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property, TValue value);
		IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property, Func<TValue> factory);
		IInterceptExpression<T> Set<TValue>(Expression<Func<T, TValue>> property, Func<IContext, TValue> factory);
		IInterceptExpression<T> TrySet(string property);
		IInterceptExpression<T> TrySet<TValue>(Expression<Func<T, TValue>> property);
	}
}