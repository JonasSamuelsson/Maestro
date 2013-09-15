using System;

namespace Maestro.Fluent
{
	public interface IConditionalInstanceBuilder<TPlugin>
	{
		IProviderSelector<TPlugin> If(Func<IContext, bool> predicate);
		IProviderSelector<TPlugin> Else { get; }
	}
}