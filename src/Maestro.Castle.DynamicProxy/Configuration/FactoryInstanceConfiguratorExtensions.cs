using System;
using Castle.DynamicProxy;
using Maestro.Castle.DynamicProxy;

namespace Maestro.Configuration
{
	public static class FactoryInstanceConfiguratorExtensions
	{
		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="factoryInstanceConfigurator"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IFactoryInstanceConfigurator<TOut> Proxy<TInstance, TOut>(this IFactoryInstanceConfigurator<TInstance> factoryInstanceConfigurator,
			Func<TInstance, ProxyGenerator, TOut> proxyFactory)
		{
			return factoryInstanceConfigurator
				.Intercept(new ProxyInterceptor<TInstance, TOut>((instance, context, proxyGenerator) => proxyFactory(instance, proxyGenerator)))
				.As<TOut>();
		}

		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="factoryInstanceConfigurator"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IFactoryInstanceConfigurator<TOut> Proxy<TInstance, TOut>(this IFactoryInstanceConfigurator<TInstance> factoryInstanceConfigurator,
			Func<TInstance, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			return factoryInstanceConfigurator
				.Intercept(new ProxyInterceptor<TInstance, TOut>(proxyFactory))
				.As<TOut>();
		}
	}
}