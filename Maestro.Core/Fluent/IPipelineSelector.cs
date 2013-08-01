using System;

namespace Maestro.Fluent
{
	public interface IPipelineSelector
	{
		ITypeInstancePipelineBuilder Type(Type type);
	}

	public interface IPipelineSelector<TPlugin>
	{
		ITypeInstancePipelineBuilder<TInstance> Type<TInstance>() where TInstance : TPlugin;
	}
}