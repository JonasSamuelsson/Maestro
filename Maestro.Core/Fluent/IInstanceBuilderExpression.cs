using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Fluent
{
	public interface IInstanceBuilderExpression<TInstance>
	{
		ILifetimeExpression<IInstanceBuilderExpression<TInstance>> Lifetime { get; }

		IInstanceBuilderExpression<TInstance> AsSingleton();
		IInstanceBuilderExpression<TInstance> AsTransient();
		IInstanceBuilderExpression<TInstance> Execute(Action<TInstance> action);
		IInstanceBuilderExpression<TInstance> Execute(Action<TInstance, IContext> action);
		IInstanceBuilderExpression<TInstance> InterceptWith(IInterceptor interceptor);
		IInstanceBuilderExpression<TOut> InterceptWith<TOut>(IInterceptor<TInstance, TOut> interceptor);
		IInstanceBuilderExpression<TOut> InterceptWith<TOut>(Func<TInstance, TOut> lambda);
		IInstanceBuilderExpression<TOut> InterceptWith<TOut>(Func<TInstance, IContext, TOut> lambda);
		IInstanceBuilderExpression<TInstance> Set(string property);
		IInstanceBuilderExpression<TInstance> Set(string property, object value);
		IInstanceBuilderExpression<TInstance> Set(string property, Func<object> factory);
		IInstanceBuilderExpression<TInstance> Set(string property, Func<IContext, object> factory);
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property);
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value);
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory);
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory);
		IInstanceBuilderExpression<TInstance> TrySet(string property);
		IInstanceBuilderExpression<TInstance> TrySet<TValue>(Expression<Func<TInstance, TValue>> property);
	}
}