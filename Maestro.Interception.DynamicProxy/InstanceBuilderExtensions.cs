using Castle.DynamicProxy;
using Maestro.Fluent;
using System;

namespace Maestro
{
	public static class InstanceBuilderExtensions
	{
		public static IInstanceBuilder<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilder<TIn> builder,
			Func<Input<TIn>, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builder.InterceptWith((i, ctx) => proxyFactory(new Input<TIn>(i, ctx, proxyGenerator)));
		}

		public static IInstanceBuilder<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilder<TIn> builder,
			Func<TIn, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builder.InterceptWith((i, ctx) => proxyFactory(i, proxyGenerator));
		}

		public static IInstanceBuilder<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilder<TIn> builder,
			Func<TIn, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builder.InterceptWith((i, ctx) => proxyFactory(i, ctx, proxyGenerator));
		}
	}
}