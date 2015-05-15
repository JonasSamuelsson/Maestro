using System;

namespace Maestro.Configuration
{
	public interface IInstanceExpression<TPlugin>
	{
		/// <summary>
		/// Setup a constant instance.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="instance"></param>
		void Use<TInstance>(TInstance instance) where TInstance : TPlugin;

		/// <summary>
		/// Setup <paramref name="lambda"/> to provide the instance.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="lambda"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin;

		/// <summary>
		/// Setup <paramref name="lambda"/> to provide the instance.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="lambda"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Use<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin;

		/// <summary>
		/// Setup type <typeparamref name="TInstance"/>.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Use<TInstance>() where TInstance : TPlugin;

		/// <summary>
		/// Setup type <paramref name="type"/>
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TPlugin> Use(Type type);

		/// <summary>
		/// Setup a constant instance.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="instance"></param>
		void Add<TInstance>(TInstance instance) where TInstance : TPlugin;

		/// <summary>
		/// Setup <paramref name="lambda"/> to provide the instance.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="lambda"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Add<TInstance>(Func<TInstance> lambda) where TInstance : TPlugin;

		/// <summary>
		/// Setup <paramref name="lambda"/> to provide the instance.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <param name="lambda"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Add<TInstance>(Func<IContext, TInstance> lambda) where TInstance : TPlugin;

		/// <summary>
		/// Setup type <typeparamref name="TInstance"/>.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <returns></returns>
		IInstanceBuilderExpression<TInstance> Add<TInstance>() where TInstance : TPlugin;

		/// <summary>
		/// Setup type <paramref name="type"/>
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IInstanceBuilderExpression<TPlugin> Add(Type type);
	}
}