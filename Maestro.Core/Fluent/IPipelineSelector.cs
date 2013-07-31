using System;

namespace Maestro.Fluent
{
	public interface IPipelineSelector
	{
		ITypePipelineBuilder Type(Type type);
	}
}