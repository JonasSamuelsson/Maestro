using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IDefaultPipelineSelector Default(Type type);
		IDefaultPipelineSelector<TPlugin> Default<TPlugin>();
		IPipelineSelector Add(Type type, string name = null);
		IPipelineSelector<TPlugin> Add<TPlugin>(string name = null);
	}
}