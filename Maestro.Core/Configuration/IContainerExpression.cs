using System;

namespace Maestro.Configuration
{
	public interface IContainerExpression
	{
		/// <summary>
		/// Used to configure instance, lifetime, interception etc for default instance of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IInstanceFactoryExpression<object> For(Type type);

		/// <summary>
		/// Used to configure instance, lifetime, interception etc for type <paramref name="type"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		IInstanceFactoryExpression<object> For(Type type, string name);

		/// <summary>
		/// Used to configure instance, lifetime, interception etc for for default instance of type <typeparamref name="TPlugin"/>.
		/// </summary>
		/// <typeparam name="TPlugin"></typeparam>
		/// <returns></returns>
		IInstanceFactoryExpression<TPlugin> For<TPlugin>();

		/// <summary>
		/// Used to configure instance, lifetime, interception etc for type <typeparamref name="TPlugin"/> named <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="TPlugin"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		IInstanceFactoryExpression<TPlugin> For<TPlugin>(string name);

		/// <summary>
		/// Used to configure instance, lifetime, interception etc for anonymous instance of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IInstanceFactoryExpression<object> Add(Type type);

		/// <summary>
		/// Used to configure instance, lifetime, interception etc for anonymous instance of type <typeparamref name="TPlugin"/>.
		/// </summary>
		/// <typeparam name="TPlugin"></typeparam>
		/// <returns></returns>
		IInstanceFactoryExpression<TPlugin> Add<TPlugin>();

		/// <summary>
		/// Used for conventional configuration.
		/// </summary>
		IConventionExpression Scan { get; }

		/// <summary>
		/// Used to setup default convention filters, lifetimes etc.
		/// </summary>
		IDefaultSettingsExpression Default { get; }
	}
}