using System;

namespace Maestro.Configuration
{
	public interface IConditionalInstanceBuilderExpression<TPlugin>
	{
		IInstanceFactoryExpression<TPlugin> If(Func<IContext, bool> predicate);
		IInstanceFactoryExpression<TPlugin> Else { get; }
	}
}