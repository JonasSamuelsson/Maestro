using System;

namespace Maestro.Fluent
{
	public interface IPipelineSelector
	{
		ITypeInstancePipelineBuilder Type(Type type);
	}
}