using System;

namespace Maestro.Fluent
{
	public interface IPipelineSelector
	{
		ITypeInstancePipelineBuilder As(Type type);
	}

	public interface IPipelineSelector<TPlugin>
	{
		ITypeInstancePipelineBuilder<TInstance> As<TInstance>() where TInstance : TPlugin;
	}
}