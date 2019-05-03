using System;
using Castle.DynamicProxy;

namespace Maestro.Castle.DynamicProxy
{
	public static class ProxyGeneratorFactory
	{
		private static readonly Lazy<ProxyGenerator> ProxyGenerator = new Lazy<ProxyGenerator>(() => new ProxyGenerator());

		/// <summary>
		/// Returns a static ProxyGenerator instance.
		/// </summary>
		/// <returns></returns>
		public static ProxyGenerator GetProxyGenerator()
		{
			return ProxyGenerator.Value;
		}
	}
}