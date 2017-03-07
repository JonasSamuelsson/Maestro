using System;
using Castle.DynamicProxy;

namespace Maestro.Configuration
{
	public static class TypeInstanceExpressionExtensions
	{
		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="typeInstanceExpression"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static ITypeInstanceExpression<TOut> Proxy<TInstance, TOut>(this ITypeInstanceExpression<TInstance> typeInstanceExpression,
			Func<TInstance, ProxyGenerator, TOut> proxyFactory)
		{
			return typeInstanceExpression
				.Intercept(new ProxyInterceptor<TInstance, TOut>((instance, context, proxyGenerator) => proxyFactory(instance, proxyGenerator)))
				.As<TOut>();
		}

		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="typeInstanceExpression"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static ITypeInstanceExpression<TOut> Proxy<TInstance, TOut>(this ITypeInstanceExpression<TInstance> typeInstanceExpression,
			Func<TInstance, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			return typeInstanceExpression
				.Intercept(new ProxyInterceptor<TInstance, TOut>(proxyFactory))
				.As<TOut>();
		}
	}
}