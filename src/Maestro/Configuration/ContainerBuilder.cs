using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public class ContainerBuilder
	{
		private readonly List<Action<IContainerExpression>> _actions = new List<Action<IContainerExpression>>();

		public ContainerBuilder() { }

		public ContainerBuilder(Action<IContainerExpression> action)
		{
			Configure(action);
		}

		public ContainerBuilder(ContainerBuilder builder)
		{
			Configure(builder);
		}

		public void Configure(Action<IContainerExpression> action)
		{
			_actions.Add(action);
		}

		public void Configure(ContainerBuilder builder)
		{
			builder._actions.ForEach(Configure);
		}

		internal void Configure(IContainer container)
		{
			_actions.ForEach(container.Configure);
		}
	}
}