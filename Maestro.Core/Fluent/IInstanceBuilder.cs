using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Fluent
{
	public interface IInstanceBuilder<TInstance>
	{
		ILifetimeExpression<IInstanceBuilder<TInstance>> Lifetime { get; }

		IInstanceBuilder<TInstance> AsSingleton();
		IInstanceBuilder<TInstance> AsTransient();
		IInstanceBuilder<TInstance> Execute(Action<TInstance> action);
		IInstanceBuilder<TInstance> Execute(Action<TInstance, IContext> action);
		IInstanceBuilder<TInstance> InterceptWith(IInterceptor interceptor);
		IInstanceBuilder<TOut> InterceptWith<TOut>(IInterceptor<TInstance, TOut> interceptor);
		IInstanceBuilder<TOut> InterceptWith<TOut>(Func<TInstance, TOut> lambda);
		IInstanceBuilder<TOut> InterceptWith<TOut>(Func<TInstance, IContext, TOut> lambda);
		IInstanceBuilder<TInstance> Set(string property);
		IInstanceBuilder<TInstance> Set(string property, object value);
		IInstanceBuilder<TInstance> Set(string property, Func<object> factory);
		IInstanceBuilder<TInstance> Set(string property, Func<IContext, object> factory);
		IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property);
		IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value);
		IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory);
		IInstanceBuilder<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory);
		IInstanceBuilder<TInstance> TrySet(string property);
		IInstanceBuilder<TInstance> TrySet<TValue>(Expression<Func<TInstance, TValue>> property);
	}
}