using System;
using Castle.DynamicProxy;
using Maestro.Castle.DynamicProxy;

namespace Maestro.Configuration
{
	public static class TypeInstanceExpressionExtensions
	{
		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="typeInstanceBuilderion"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static ITypeInstanceExpression<TOut> Proxy<TInstance, TOut>(this ITypeInstanceExpression<TInstance> typeInstanceBuilder,
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
		/// <param name="typeInstanceBuilderion"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static ITypeInstanceExpression<TOut> Proxy<TInstance, TOut>(this ITypeInstanceExpression<TInstance> typeInstanceBuilder,
			Func<TInstance, Context, ProxyGenerator, TOut> proxyFactory)
		{
			return typeInstanceBuilder
				.Intercept(new ProxyInterceptor<TInstance, TOut>(proxyFactory))
				.As<TOut>();
		}
	}
}