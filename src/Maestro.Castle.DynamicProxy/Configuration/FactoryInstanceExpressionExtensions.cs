﻿using System;
using Castle.DynamicProxy;

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
			return factoryInstanceExpression.Intercept<TOut>(new ProxyInterceptor<TInstance, TOut>((instance, context, proxyGenerator) => proxyFactory(instance, proxyGenerator)));
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
			Func<TInstance, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			return factoryInstanceExpression.Intercept<TOut>(new ProxyInterceptor<TInstance, TOut>(proxyFactory));
		}
	}
}