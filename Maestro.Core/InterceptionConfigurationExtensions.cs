using Maestro.Fluent;
using Maestro.Interceptors;
using System;
using System.Linq.Expressions;

namespace Maestro
{
	public static class InterceptionConfigurationExtensions
	{
		public static TParent SetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property)
		{
			return parent.InterceptUsing(new SetPropertyInterceptor(property));
		}

		public static TParent SetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, object value)
		{
			return parent.SetProperty(property, _ => value);
		}

		public static TParent SetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, Func<object> factory)
		{
			return parent.SetProperty(x => factory());
		}

		public static TParent SetProperty<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, Func<IContext, object> factory)
		{
			return parent.InterceptUsing(new SetPropertyInterceptor(property, factory));
		}

		public static TParent SetProperty<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.SetProperty(name);
		}

		public static TParent SetProperty<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, TValue value)
		{
			return parent.SetProperty(property, _ => value);
		}

		public static TParent SetProperty<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			return parent.SetProperty(property, _ => factory());
		}

		public static TParent SetProperty<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.SetProperty(name, x => factory(x));
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