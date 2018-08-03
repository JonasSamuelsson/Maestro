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
		public Container(Action<ContainerBuilder> action)
			: this()
		{
			Configure(action);
		}

		public void Configure(Action<ContainerBuilder> action)
		{
			var containerExpression = new ContainerBuilder(this);
			try
			{
				action(containerExpression);
			}
			finally
			{
				containerExpression.Dispose();
			}
		}
	}
}