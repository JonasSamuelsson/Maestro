using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro.Configuration
{
	public interface IInstanceExpression<TInstance, TParent>
	{
		ILifetimeSelector<TParent> Lifetime { get; }

		/// <summary>
		/// Adds an action to execute against the instance.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		TParent Intercept(Action<TInstance> interceptor);

		/// <summary>
		/// Adds an action to execute against the instance.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		TParent Intercept(Action<TInstance, Context> interceptor);

		/// <summary>
		/// Adds a func that processes the instance.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		TParent Intercept(Func<TInstance, TInstance> interceptor);

		/// <summary>
		/// Adds a func that processes the instance.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		TParent Intercept(Func<TInstance, Context, TInstance> interceptor);

		/// <summary>
		/// Adds <paramref name="interceptor"/> to the pipeline.
		/// </summary>
		/// <param name="interceptor"></param>
		/// <returns></returns>
		TParent Intercept(IInterceptor interceptor);

		/// <summary>
		/// Set property <paramref name="property"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Throws if the property type can't be resolved.</remarks>
		TParent SetProperty(string property, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw);

		/// <summary>
		/// Set property <paramref name="property"/> with value <paramref name="value"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		TParent SetProperty(string property, object value, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		TParent SetProperty(string property, Func<object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		TParent SetProperty(string property, Func<Context, object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw);

		TParent SetProperty(string property, Func<Context, Type, object> factory, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw);
		
		/// <summary>
		/// Set property <paramref name="property"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <remarks>Throws if the property type can't be resolved.</remarks>
		TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property);

		/// <summary>
		/// Set property <paramref name="property"/> with value <paramref name="value"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, TValue value);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, Func<TValue> factory);

		/// <summary>
		/// Set property <paramref name="property"/> with value from <paramref name="factory"/>.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="property"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		TParent SetProperty<TValue>(Expression<Func<TInstance, TValue>> property, Func<Context, TValue> factory);

		TParent TrySetProperty(string property, PropertyNotFoundAction propertyNotFoundAction = PropertyNotFoundAction.Throw);

		TParent TrySetProperty<TValue>(Expression<Func<TInstance, TValue>> property);
}
}