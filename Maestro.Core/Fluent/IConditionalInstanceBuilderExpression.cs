using System;

namespace Maestro.Fluent
{
	public interface IConditionalInstanceBuilderExpression<TPlugin>
	{
		IInstanceFactoryExpression<TPlugin> If(Func<IContext, bool> predicate);
		IInstanceFactoryExpression<TPlugin> Else { get; }
	}
}