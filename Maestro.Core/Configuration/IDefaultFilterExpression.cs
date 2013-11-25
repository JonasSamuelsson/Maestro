using System;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public interface IDefaultFilterExpression
	{
		/// <summary>
		/// Adds <paramref name="predicate"/> to the list of global filters.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IDefaultSettingsExpression Add(Func<Type, bool> predicate);

		/// <summary>
		/// Adds <paramref name="filter"/> to the list of global filters.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		IDefaultSettingsExpression Add(IConventionFilter filter);

		/// <summary>
		/// Clears the list of global filters.
		/// </summary>
		/// <returns></returns>
		IDefaultSettingsExpression Clear();
	}
}