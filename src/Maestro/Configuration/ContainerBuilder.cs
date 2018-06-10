using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public class ContainerBuilder
	{
		private readonly List<Action<ContainerExpression>> _actions = new List<Action<ContainerExpression>>();

		public ContainerBuilder() { }

		public ContainerBuilder(Action<ContainerExpression> action)
		{
			Configure(action);
		}

		public ContainerBuilder(ContainerBuilder builder)
		{
			Configure(builder);
		}

		public void Configure(Action<ContainerExpression> action)
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