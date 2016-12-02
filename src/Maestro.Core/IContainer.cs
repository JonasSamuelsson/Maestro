using System;
using System.Collections.Generic;
using Maestro.Configuration;

namespace Maestro
{
	public interface IContainer : IDisposable
	{
		/// <summary>
		/// Adds configuration to the container.
		/// </summary>
		/// <param name="action"></param>
		void Configure(Action<IContainerExpression> action);

		IContainer GetChildContainer();
		IContainer GetChildContainer(Action<IContainerExpression> action);

		/// <summary>
		/// Gets an instance of type <paramref name="type"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name">Uses the default instance if a named instance isn't found.</param>
		/// <returns></returns>
		object GetService(Type type, string name = null);

		/// <summary>
		/// Gets an instance of type <typeparamref name="T"/> named <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">Uses the default instance if a named instance isn't found.</param>
		/// <returns></returns>
		T GetService<T>(string name = null);

		/// <summary>
		/// Gets all instances of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IEnumerable<object> GetServices(Type type);

		/// <summary>
		/// Gets all instances of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<T> GetServices<T>();

		/// <summary>
		/// Tries to get instance of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		/// <remarks>Does not swallow any exceptions.</remarks>
		bool TryGetService(Type type, out object instance);

		/// <summary>
		/// Tries to get instance of type <paramref name="type"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		/// <remarks>Does not swallow any exceptions.</remarks>
		bool TryGetService(Type type, string name, out object instance);

		/// <summary>
		/// Tries to get instance of type <typeparam name="T"/>.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		/// <remarks>Does not swallow any exceptions.</remarks>
		bool TryGetService<T>(out T instance);

		/// <summary>
		/// Tries to get instance of type <typeparam name="T"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		/// <remarks>Does not swallow any exceptions.</remarks>
		bool TryGetService<T>(string name, out T instance);

		/// <summary>
		/// Gets a report of the complete container configuration.
		/// </summary>
		/// <returns></returns>
		string GetConfiguration();
	}
}