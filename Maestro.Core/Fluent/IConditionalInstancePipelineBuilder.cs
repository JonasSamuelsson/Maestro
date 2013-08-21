using System;

namespace Maestro.Fluent
{
	public interface IConditionalInstancePipelineBuilder<TPlugin>
	{
		IProviderSelector<TPlugin> If(Func<IContext, bool> predicate);
		IProviderSelector<TPlugin> Default { get; }
	}
}