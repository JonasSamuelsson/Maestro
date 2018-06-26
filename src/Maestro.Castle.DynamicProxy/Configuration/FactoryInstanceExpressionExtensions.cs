﻿using System;
using Castle.DynamicProxy;
using Maestro.Castle.DynamicProxy;

namespace Maestro.Configuration
{
	public static class FactoryInstanceExpressionExtensions
	{
		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="factoryInstanceExpression"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IFactoryInstanceExpression<TOut> Proxy<TInstance, TOut>(this IFactoryInstanceExpression<TInstance> factoryInstanceExpression,
			Func<TInstance, ProxyGenerator, TOut> proxyFactory)
		{
			return factoryInstanceExpression
				.Intercept(new ProxyInterceptor<TInstance, TOut>((instance, context, proxyGenerator) => proxyFactory(instance, proxyGenerator)))
				.As<TOut>();
		}

		/// <summary>
		/// Adds a pransparent proxy interceptor to the pipeline.
		/// </summary>
		/// <typeparam name="TInstance"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <param name="factoryInstanceExpression"></param>
		/// <param name="proxyFactory"></param>
		/// <returns></returns>
		public static IFactoryInstanceExpression<TOut> Proxy<TInstance, TOut>(this IFactoryInstanceExpression<TInstance> factoryInstanceExpression,
			Func<TInstance, Context, ProxyGenerator, TOut> proxyFactory)
		{
			return factoryInstanceExpression
				.Intercept(new ProxyInterceptor<TInstance, TOut>(proxyFactory))
				.As<TOut>();
		}
	}
}