﻿using System;
using Castle.DynamicProxy;

namespace Maestro.Castle.DynamicProxy
{
	internal class ProxyInterceptor<TInstance, TOut> : Interceptors.IInterceptor
	{
		private readonly Func<TInstance, IContext, ProxyGenerator, TOut> _proxyFactory;

		public ProxyInterceptor(Func<TInstance, IContext, ProxyGenerator, TOut> proxyFactory)
		{
			_proxyFactory = proxyFactory;
		}

		public object Execute(object instance, IContext context)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return _proxyFactory.Invoke((TInstance)instance, context, proxyGenerator);
		}
	}
}