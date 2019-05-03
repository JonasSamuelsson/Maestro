using Castle.DynamicProxy;
using System;

namespace Maestro.Castle.DynamicProxy
{
	internal class ProxyInterceptor<TInstance, TOut> : Interceptors.IInterceptor
	{
		private readonly Func<TInstance, Context, ProxyGenerator, TOut> _proxyFactory;

		public ProxyInterceptor(Func<TInstance, Context, ProxyGenerator, TOut> proxyFactory)
		{
			_proxyFactory = proxyFactory;
		}

		public object Execute(object instance, Context context)
		{
			var proxyGenerator = ProxyGeneratorFactory.GetProxyGenerator();
			return _proxyFactory.Invoke((TInstance)instance, context, proxyGenerator);
		}
	}
}