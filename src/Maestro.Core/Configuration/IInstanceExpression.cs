using System;
using System.Linq.Expressions;
using Maestro.Interceptors;

namespace Maestro.Configuration
{
	public interface IInstanceExpression<TInstance, TParent>
	{
		ILifetimeExpression<TParent> Lifetime { get; }

		/// <summary>
		/// Adds an action to execute against the instance.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> Intercept(Action<TInstance> action);

		/// <summary>
		/// Adds an action to execute against the instance.
		/// </summary>
		/// <param name = "action" ></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> Intercept(Action<TInstance, IContext> action);

		/// <summary>
		/// Adds a func to execute against the instance.
		/// </summary>
		/// <param name="func"></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> Intercept(Func<TInstance, TInstance> func);

		/// <summary>
		/// Adds a func to execute against the instance.
		/// </summary>
		/// <param name = "func" ></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> Intercept(Func<TInstance, IContext, TInstance> func);

		/// <summary>
		/// Adds <paramref name="interceptor"/> to the pipeline.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> Intercept(IInterceptor interceptor);

		///// <summary>
		///// Adds <paramref name="interceptor"/> to the pipeline.
		///// </summary>
		///// <typeparam name="TOut"></typeparam>
		///// <param name="interceptor"></param>
		///// <returns></returns>
		//IInstanceExpression<TOut, TParent> Intercept<TOut>(IInterceptor<TInstance, TOut> interceptor);

		///// <summary>
		///// Adds <paramref name="lambda"/> to the pipeline.
		///// </summary>
		///// <typeparam name="TOut"></typeparam>
		///// <param name="lambda"></param>
		///// <returns></returns>
		//IInstanceExpression<TOut, TParent> Intercept<TOut>(Func<TInstance, TOut> lambda);

		///// <summary>
		///// Adds <paramref name="lambda"/> to the pipeline.
		///// </summary>
		///// <typeparam name="TOut"></typeparam>
		///// <param name="lambda"></param>
		///// <returns></returns>
		//IInstanceExpression<TOut, TParent> Intercept<TOut>(Func<TInstance, IContext, TOut> lambda);

		/// <summary>
		/// Set property <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Throws if the property type can't be resolved.</remarks>
		IInstanceExpression<TInstance, TParent> SetProperty<TValue>(Expression<Func<TInstance, TValue>> property);

		/// <summary>
		/// Set property <paramref name="property"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Throws if the property type can't be resolved.</remarks>
		IInstanceExpression<TInstance, TParent> SetProperty(string property);

		/// <summary>
		/// Set property <paramref name="property"/> with value <paramref name="value"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IInstanceExpression<TInstance, TParent> SetProperty(string property, object value);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set(string property, Func<object> factory);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set(string property, Func<IContext, object> factory);

		///// <summary>
		///// Set property <paramref name="property"/> with value <paramref name="value"/>.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <param name="value"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory);

		///// <summary>
		///// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <param name="factory"></param>
		///// <returns></returns>
		//IInstanceExpression<TInstance, TParent> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory);

		///// <summary>
		///// Set property <paramref name="property"/> if the property type can be resolved.
		///// </summary>
		///// <param name="property"></param>
		///// <returns></returns>
		///// <remarks>Does not throw if the property type can't be resolved.</remarks>
		//IInstanceExpression<TInstance, TParent> TrySet(string property);

		///// <summary>
		///// Set property <paramref name="property"/> if the property type can be resolved.
		///// </summary>
		///// <typeparam name="TValue"></typeparam>
		///// <param name="property"></param>
		///// <returns></returns>
		///// <remarks>Does not throw if the property type can't be resolved.</remarks>
		//IInstanceExpression<TInstance, TParent> TrySet<TValue>(Expression<Func<TInstance, TValue>> property);
	}
}