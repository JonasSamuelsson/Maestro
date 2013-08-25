using Maestro.Fluent;
using System;

namespace Maestro
{
	public static class ConfigureInterceptionExtensions
	{
		public static TParent OnCreate<TParent>(this TParent expression, Action<IInterceptExpression<TParent>> action)
			where TParent : IOnCreateExpression<TParent>
		{
			action(expression.OnCreate);
			return expression;
		}

		public static TParent OnActivate<TParent>(this TParent expression, Action<IInterceptExpression<TParent>> action)
			where TParent : IOnActivateExpression<TParent>
		{
			action(expression.OnActivate);
			return expression;
		}
	}
}