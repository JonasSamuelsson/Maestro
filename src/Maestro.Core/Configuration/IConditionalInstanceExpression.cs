using System;

namespace Maestro.Configuration
{
	public interface IConditionalInstanceExpression<TPlugin>
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
	}
}