using System;
using System.Linq.Expressions;
using Maestro.Interceptors;

namespace Maestro.Configuration
{
	public interface IInstanceBuilderExpression<TInstance>
	{
		/// <summary>
		/// Gives access to full lifetime configuration.
		/// </summary>
		ILifetimeExpression<IInstanceBuilderExpression<TInstance>> Lifetime { get; }

		/// <summary>
		/// Adds an action to execute against the instance.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Execute(Action<TInstance> action);

		/// <summary>
		/// Adds an action to execute against the instance.
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Execute(Action<TInstance, IContext> action);

		/// <summary>
		/// Adds <paramref name="interceptor"/> to the pipeline.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Intercept(IInterceptor interceptor);

		/// <summary>
		/// Adds <paramref name="interceptor"/> to the pipeline.
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TOut> Intercept<TOut>(IInterceptor<TInstance, TOut> interceptor);

		/// <summary>
		/// Adds <paramref name="lambda"/> to the pipeline.
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="lambda"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TOut> Intercept<TOut>(Func<TInstance, TOut> lambda);

		/// <summary>
		/// Adds <paramref name="lambda"/> to the pipeline.
		/// </summary>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="lambda"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TOut> Intercept<TOut>(Func<TInstance, IContext, TOut> lambda);

		/// <summary>
		/// Set property <paramref name="property"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Throws if the property type can't be resolved.</remarks>
		IInstanceBuilderExpression<TInstance> Set(string property);

		/// <summary>
		/// Set property <paramref name="property"/> with value <paramref name="value"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Set(string property, object value);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Set(string property, Func<object> factory);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Set(string property, Func<IContext, object> factory);

		/// <summary>
		/// Set property <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Throws if the property type can't be resolved.</remarks>
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property);

		/// <summary>
		/// Set property <paramref name="property"/> with value <paramref name="value"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, TValue value);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Set<TValue>(Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory);

		/// <summary>
		/// Set property <paramref name="property"/> if the property type can be resolved.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Does not throw if the property type can't be resolved.</remarks>
		IInstanceBuilderExpression<TInstance> TrySet(string property);

		/// <summary>
		/// Set property <paramref name="property"/> if the property type can be resolved.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Does not throw if the property type can't be resolved.</remarks>
		IInstanceBuilderExpression<TInstance> TrySet<TValue>(Expression<Func<TInstance, TValue>> property);
	}
}