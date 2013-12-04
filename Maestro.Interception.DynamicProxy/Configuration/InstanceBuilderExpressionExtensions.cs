using System;
using Castle.DynamicProxy;

namespace Maestro.Configuration
{
	public static class InstanceBuilderExpressionExtensions
	{
		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="builderExpression"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IInstanceBuilderExpression<TOut> Proxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<Input<TIn>, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.Intercept((x, ctx) => proxyFactory(new Input<TIn>(x, ctx, proxyGenerator)));
		}

		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="builderExpression"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IInstanceBuilderExpression<TOut> Proxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<TIn, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.Intercept((x, ctx) => proxyFactory(x, proxyGenerator));
		}

		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TIn"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="builderExpression"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IInstanceBuilderExpression<TOut> Proxy<TIn, TOut>(this IInstanceBuilderExpression<TIn> builderExpression,
			Func<TIn, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return builderExpression.Intercept((x, ctx) => proxyFactory(x, ctx, proxyGenerator));
		}
	}
}