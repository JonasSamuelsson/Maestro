using Maestro.Fluent;
using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro
{
	public static class InterceptionConfigurationExtensions
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

		public static TParent SetProperty<TParent>(this IInterceptExpression<TParent> parent, string property)
		{
			return parent.InterceptUsing(new SetPropertyInterceptor(property));
		}

		//public static TParent SetProperty<TParent>(this IInterceptExpression<TParent> parent, Expression<Func<object>> property)
		//{
		//	var name = ((MemberExpression)property.Body).Member.Name;
		//	return parent.InterceptUsing(new SetPropertyInterceptor(name));
		//}
	}
}