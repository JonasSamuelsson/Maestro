using System;
using Castle.DynamicProxy;
using IInterceptor = Maestro.Interceptors.IInterceptor;

namespace Maestro.Configuration
{
	internal class ProxyInterceptor<TInstance, TOut> : IInterceptor
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

		public IInterceptor MakeGeneric(Type[] genericArguments)
		{
			throw new NotImplementedException();
		}
	}
}