using Maestro.Configuration;
using System;

namespace Maestro
{
	public interface IContainer : IScopedContainer
	{
		/// <summary>
		/// Adds configuration to the container.
		/// </summary>
		/// <param name="action"></param>
		void Configure(Action<ContainerExpression> action);
		void Configure(ContainerBuilder builder);
	}
}