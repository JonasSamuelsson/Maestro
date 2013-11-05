using System;

namespace Maestro.Fluent
{
	public interface IConditionalInstanceBuilder<TPlugin>
	{
		IInstanceExpression<TPlugin> If(Func<IContext, bool> predicate);
		IInstanceExpression<TPlugin> Else { get; }
	}
}