using System;

namespace Maestro.Configuration
{
	public interface IConditionalInstanceBuilderExpression<TPlugin>
	{
		/// <summary>
		/// Used to configure the instance for when <paramref name="predicate"/> is satisfied.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IInstanceFactoryExpression<TPlugin> If(Func<IContext, bool> predicate);

		/// <summary>
		/// Used to configure the default instance.
		/// </summary>
		IInstanceFactoryExpression<TPlugin> Else { get; }
	}
}