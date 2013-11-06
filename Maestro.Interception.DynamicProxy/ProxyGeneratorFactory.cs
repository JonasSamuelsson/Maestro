using Castle.DynamicProxy;
using System;

namespace Maestro
{
	public static class ProxyGeneratorFactory
	{
		private static readonly Lazy<ProxyGenerator> ProxyGenerator = new Lazy<ProxyGenerator>(() => new ProxyGenerator());

		public static ProxyGenerator GetProxyGenerator()
		{
			return ProxyGenerator.Value;
		}
	}
}