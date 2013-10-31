﻿using System;
using System.Linq.Expressions;
using Maestro.Fluent;
using Maestro.Interceptors;

namespace Maestro
{
	public static class InterceptionConfigurationExtensions
	{
		public static TParent Decorate<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Action<TInstance> action)
		{
			return parent.Decorate((instance, context) => action(instance));
		}

		public static TParent Decorate<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Action<TInstance, IContext> action)
		{
			return parent.Intercept((i, c) =>
											{
												action(i, c);
												return i;
											});
		}

		public static TParent Intercept<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Func<TInstance, TInstance> func)
		{
			return parent.Intercept((instance, context) => func(instance));
		}

		public static TParent Intercept<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Func<TInstance, IContext, TInstance> func)
		{
			return parent.Intercept(new LambdaInterceptor<TInstance>(func));
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property)
		{
			return parent.Intercept(new SetPropertyInterceptor(property));
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, object value)
		{
			return parent.Set(property, _ => value);
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, Func<object> factory)
		{
			return parent.Set(property, x => factory());
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property, Func<IContext, object> factory)
		{
			return parent.Intercept(new SetPropertyInterceptor(property, factory));
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.Set(name);
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, TValue value)
		{
			return parent.Set(property, _ => value);
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, Func<TValue> factory)
		{
			return parent.Set(property, _ => factory());
		}

		public static TParent Set<TInstance, TValue, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, TValue>> property, Func<IContext, TValue> factory)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.Set(name, x => factory(x));
		}

		public static TParent Set<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, object>> property)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.Set(name);
		}

		public static TParent TrySet<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, string property)
		{
			return parent.Intercept(new TrySetPropertyInterceptor(property));
		}

		public static TParent TrySet<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent, Expression<Func<TInstance, object>> property)
		{
			var name = ((MemberExpression)property.Body).Member.Name;
			return parent.TrySet(name);
		}
	}
}