using System;
using Castle.DynamicProxy;

namespace Maestro.Configuration
{
	public static class InstanceBuilderExpressionExtensions
	{
		public static IInstanceBuilderExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<Input<TIn>, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.InterceptWith((x, ctx) => proxyFactory(new Input<TIn>(x, ctx, proxyGenerator)));
		}

		public static IInstanceBuilderExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<TIn, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.InterceptWith((x, ctx) => proxyFactory(x, proxyGenerator));
		}

		public static IInstanceBuilderExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<TIn, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.InterceptWith((x, ctx) => proxyFactory(x, ctx, proxyGenerator));
		}
	}
}