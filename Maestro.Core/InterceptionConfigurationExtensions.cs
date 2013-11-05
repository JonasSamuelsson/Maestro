using System;
using System.Linq.Expressions;
using Maestro.Fluent;
using Maestro.Interceptors;

namespace Maestro
{
	public static class InterceptionConfigurationExtensions
	{
		public static TParent Execute<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Action<TInstance> action)
		{
			return parent.Execute((instance, context) => action(instance));
		}

		public static TParent Execute<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Action<TInstance, IContext> action)
		{
			return parent.InterceptWith((i, c) =>
											{
												action(i, c);
												return i;
											});
		}

		public static TParent InterceptWith<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Func<TInstance, TInstance> func)
		{
			return parent.InterceptWith((instance, context) => func(instance));
		}

		public static TParent InterceptWith<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Func<TInstance, IContext, TInstance> func)
		{
			return parent.InterceptWith(new LambdaInterceptor<TInstance>(func));
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property)
		{
			return parent.InterceptWith(new SetPropertyInterceptor(property));
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, object value)
		{
			return parent.InterceptWith(new SetPropertyInterceptor(property, _ => value));
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, Func<object> factory)
		{
			return parent.InterceptWith(new SetPropertyInterceptor(property, _ => factory()));
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, Func<IContext, object> factory)
		{
			return parent.InterceptWith(new SetPropertyInterceptor(property, factory));
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property)
		{
			return parent.Set(property.GetName());
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, TValue value)
		{
			var propertyName = property.GetName();
			return parent.InterceptWith(new SetPropertyInterceptor(propertyName, _ => value));
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			var propertyName = property.GetName();
			return parent.InterceptWith(new SetPropertyInterceptor(propertyName, _ => factory()));
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			var propertyName = property.GetName();
			return parent.InterceptWith(new SetPropertyInterceptor(propertyName, ctx => factory(ctx)));
		}

		public static TParent TrySet<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property)
		{
			return parent.InterceptWith(new TrySetPropertyInterceptor(property));
		}

		public static TParent TrySet<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, object>> property)
		{
			return parent.TrySet(property.GetName());
		}

		private static string GetName<TInstance, TValue>(this Expression<Func<TInstance, TValue>> property)
		{
			return ((MemberExpression)property.Body).Member.Name;
		}
	}
}