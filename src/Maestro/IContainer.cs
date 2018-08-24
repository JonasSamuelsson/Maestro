using Maestro.Configuration;
using System;

namespace Maestro
{
	public interface IContainer : IScope
	{
		/// <summary>
		/// Adds configuration to the container.
		/// </summary>
		/// <param name="action"></param>
		void Configure(Action<IContainerBuilder> action);
	}
}