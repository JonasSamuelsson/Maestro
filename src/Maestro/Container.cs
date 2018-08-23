using Maestro.Configuration;
using System;

namespace Maestro
{
	public class Container : ScopedContainer, IContainer
	{
		/// <summary>
		/// Instantiates a new empty container.
		/// </summary>
		public Container()
		{
		}

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(Action<IContainerBuilder> action)
			: this()
		{
			Configure(action);
		}

		public void Configure(Action<IContainerBuilder> action)
		{
			action.Invoke(new InternalContainerBuilder(this));
		}
	}
}