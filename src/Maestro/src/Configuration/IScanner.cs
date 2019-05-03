using Maestro.Conventions;
using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public interface IScanner
	{
		/// <summary>
		/// Adds provided types to the list of types to process.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		IScanner Types(IEnumerable<Type> types);

		/// <summary>
		/// Filter types to those matching <paramref name="predicate"/>.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IScanner Where(Func<Type, bool> predicate);

		/// <summary>
		/// Uses <paramref name="convention"/> to configure the container.
		/// </summary>
		/// <param name="convention"></param>
		IScanner Using(IConvention convention);
	}
}