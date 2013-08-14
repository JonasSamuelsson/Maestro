using System;

namespace Maestro.Fluent
{
	public interface IConditionalInstancePipelineBuilder<TPlugin>
	{
		IPipelineSelector<TPlugin> If(Func<IContext, bool> predicate);
		IPipelineSelector<TPlugin> Default { get; }
	}
}