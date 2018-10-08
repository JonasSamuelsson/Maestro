using Castle.DynamicProxy;
using Maestro.Castle.DynamicProxy;
using System;

namespace Maestro.Configuration
{
	public static class FactoryInstanceBuilderExtensions
	{
		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="factoryInstanceBuilder"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IFactoryInstanceBuilder<TOut> Proxy<TInstance, TOut>(this IFactoryInstanceBuilder<TInstance> factoryInstanceBuilder,
			Func<TInstance, ProxyGenerator, TOut> proxyFactory)
		{
			return factoryInstanceBuilder
				.Intercept(new ProxyInterceptor<TInstance, TOut>((instance, context, proxyGenerator) => proxyFactory(instance, proxyGenerator)))
				.As<TOut>();
		}

		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="factoryInstanceBuilder"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IFactoryInstanceBuilder<TOut> Proxy<TInstance, TOut>(this IFactoryInstanceBuilder<TInstance> factoryInstanceBuilder,
			Func<TInstance, Context, ProxyGenerator, TOut> proxyFactory)
		{
			return factoryInstanceBuilder
				.Intercept(new ProxyInterceptor<TInstance, TOut>(proxyFactory))
				.As<TOut>();
		}
	}
}