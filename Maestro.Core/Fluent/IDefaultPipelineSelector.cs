using System;

namespace Maestro.Fluent
{
	public interface IDefaultPipelineSelector
	{
		ITypeInstancePipelineBuilder Is(Type type);
	}

	public interface IDefaultPipelineSelector<TPlugin>
	{
		ITypeInstancePipelineBuilder<TInstance> Is<TInstance>() where TInstance : TPlugin;
	}
}