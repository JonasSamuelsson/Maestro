using System;
using System.Linq.Expressions;
using Maestro.Fluent;
using Maestro.Interceptors;

namespace Maestro
{
	public static class InterceptionConfigurationExtensions
	{
		public static TParent SetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property)
		{
			return parent.InterceptUsing(new SetPropertyInterceptor(property));
		}

		public static TParent SetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, object>> property)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.SetProperty(name);
		}

		public static TParent TrySetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property)
		{
			return parent.InterceptUsing(new TrySetPropertyInterceptor(property));
		}

		public static TParent TrySetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, object>> property)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.TrySetProperty(name);
		}
	}
}