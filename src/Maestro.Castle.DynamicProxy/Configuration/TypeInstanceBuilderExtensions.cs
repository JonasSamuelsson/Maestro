using Castle.DynamicProxy;
using Maestro.Castle.DynamicProxy;
using System;

namespace Maestro.Configuration
{
	public static class TypeInstanceBuilderExtensions
	{
		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="typeInstanceBuilder"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static ITypeInstanceBuilder<TOut> Proxy<TInstance, TOut>(this ITypeInstanceBuilder<TInstance> typeInstanceBuilder,
			Func<TInstance, ProxyGenerator, TOut> proxyFactory)
		{
			return typeInstanceBuilder
				.Intercept(new ProxyInterceptor<TInstance, TOut>((instance, context, proxyGenerator) => proxyFactory(instance, proxyGenerator)))
				.As<TOut>();
		}

		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="typeInstanceBuilder"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static ITypeInstanceBuilder<TOut> Proxy<TInstance, TOut>(this ITypeInstanceBuilder<TInstance> typeInstanceBuilder,
			Func<TInstance, Context, ProxyGenerator, TOut> proxyFactory)
		{
			return typeInstanceBuilder
				.Intercept(new ProxyInterceptor<TInstance, TOut>(proxyFactory))
				.As<TOut>();
		}
	}
}