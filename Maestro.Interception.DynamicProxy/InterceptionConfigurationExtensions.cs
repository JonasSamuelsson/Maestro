using System;
using Castle.DynamicProxy;
using Maestro.Fluent;

namespace Maestro
{
	public static class InterceptionConfigurationExtensions
	{
		private static readonly Lazy<ProxyGenerator> ProxyGenerator = new Lazy<ProxyGenerator>(() => new ProxyGenerator());

		public static TParent InterceptWithProxy<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent,
			Func<TInstance, ProxyGenerator, TInstance> proxyFactory)
		{
			var proxyGenerator = GetProxyGenerator();
			return parent.InterceptWith((i, ctx) => proxyFactory(i, proxyGenerator));
		}

		public static TParent InterceptWithProxy<TInstance, TParent>(this IInterceptExpression<TInstance, TParent> parent,
			Func<TInstance, IContext, ProxyGenerator, TInstance> proxyFactory)
		{
			var proxyGenerator = GetProxyGenerator();
			return parent.InterceptWith((i, ctx) => proxyFactory(i, ctx, proxyGenerator));
		}

		private static ProxyGenerator GetProxyGenerator()
		{
			return ProxyGenerator.Value;
		}
	}
}