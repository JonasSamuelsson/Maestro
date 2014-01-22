using System;

namespace Maestro.Configuration
{
	public interface IConditionalExpression<TPlugin>
	{
		/// <summary>
		/// Used to configure the instance for when <paramref name="predicate"/> is satisfied.
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IConditionalInstanceExpression<TPlugin> If(Func<IContext, bool> predicate);

		/// <summary>
		/// Used to configure the default instance.
		/// </summary>
		IConditionalInstanceExpression<TPlugin> Else { get; }
	}
}