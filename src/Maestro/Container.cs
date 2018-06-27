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

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(ContainerBuilder builder)
			: this()
		{
			builder.Configure(this);
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

		public void Configure(ContainerBuilder builder)
		{
			builder.Configure(this);
		}
	}
}