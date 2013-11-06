using System;
using Castle.DynamicProxy;
using Maestro.Fluent;

namespace Maestro
{
	public static class InterceptExpressionExtensions
	{
		public static IInterceptExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInterceptExpression<TIn> expression,
		Func<Input<TIn>, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return expression.InterceptWith((i, ctx) => proxyFactory(new Input<TIn>(i, ctx, proxyGenerator)));
		}

		public static IInterceptExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInterceptExpression<TIn> expression,
			Func<TIn, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return expression.InterceptWith((i, ctx) => proxyFactory(i, proxyGenerator));
		}

		public static IInterceptExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInterceptExpression<TIn> expression,
			Func<TIn, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return expression.InterceptWith((i, ctx) => proxyFactory(i, ctx, proxyGenerator));
		}

	}
}