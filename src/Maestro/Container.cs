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
		public Container(Action<ContainerExpression> action)
			: this()
		{
			Configure(action);
		}

		public void Configure(Action<ContainerExpression> action)
		{
			var containerExpression = new ContainerExpression(this);
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