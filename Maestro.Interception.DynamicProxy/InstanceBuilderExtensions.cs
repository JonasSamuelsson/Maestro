using Castle.DynamicProxy;
using Maestro.Fluent;
using System;

namespace Maestro
{
	public static class InstanceBuilderExtensions
	{
		public static IInstanceBuilderExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<Input<TIn>, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.InterceptWith((i, ctx) => proxyFactory(new Input<TIn>(i, ctx, proxyGenerator)));
		}

		public static IInstanceBuilderExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<TIn, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.InterceptWith((i, ctx) => proxyFactory(i, proxyGenerator));
		}

		public static IInstanceBuilderExpression<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<TIn, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.InterceptWith((i, ctx) => proxyFactory(i, ctx, proxyGenerator));
		}
	}
}