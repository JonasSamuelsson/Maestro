using System;
using Castle.DynamicProxy;
using Maestro.Fluent;

namespace Maestro
{
	public static class InstanceBuilderExtensions
	{
		private static readonly Lazy<ProxyGenerator> ProxyGenerator = new Lazy<ProxyGenerator>(() => new ProxyGenerator());

		public static IInstanceBuilder<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilder<TIn> builder,
			Func<Input<TIn>, TOut> proxyFactory)
		{
			var proxyGenerator = GetProxyGenerator();
			return builder.InterceptWith((i, ctx) => proxyFactory(new Input<TIn>(i, ctx, proxyGenerator)));
		}
		
		public static IInstanceBuilder<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilder<TIn> builder,
			Func<TIn, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = GetProxyGenerator();
			return builder.InterceptWith((i, ctx) => proxyFactory(i, proxyGenerator));
		}
		
		public static IInstanceBuilder<TOut> InterceptWithProxy<TIn, TOut>(this IInstanceBuilder<TIn> builder,
			Func<TIn, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = GetProxyGenerator();
			return builder.InterceptWith((i, ctx) => proxyFactory(i, ctx, proxyGenerator));
		}

		private static ProxyGenerator GetProxyGenerator()
		{
			return ProxyGenerator.Value;
		}
	}
}