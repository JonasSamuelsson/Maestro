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

		/// <summary>
		/// Gets an instance of type <paramref name="type"/> named <paramref name="name"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name">Uses the default instance if a named instance isn't found.</param>
		/// <returns></returns>
		object Get(Type type, string name = null);

		/// <summary>
		/// Gets an instance of type <typeparamref name="T"/> named <paramref name="name"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name">Uses the default instance if a named instance isn't found.</param>
		/// <returns></returns>
		T Get<T>(string name = null);

		/// <summary>
		/// Gets all instances of type <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IEnumerable<object> GetAll(Type type);

		/// <summary>
		/// Gets all instances of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<T> GetAll<T>();

		/// <summary>
		/// Gets a report of the complete container configuration.
		/// </summary>
		/// <returns></returns>
		string GetConfiguration();
	}
}