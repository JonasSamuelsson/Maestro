using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IPipelineSelector Default(Type type);
		IPipelineSelector<TPlugin> Default<TPlugin>();
		IPipelineSelector<TPlugin> Add<TPlugin>(string name = null);
	}
}