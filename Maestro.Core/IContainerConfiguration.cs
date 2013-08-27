using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IProviderSelector For(Type type);
		IProviderSelector<TPlugin> For<TPlugin>();
		IProviderSelector Add(Type type, string name = null);
		IProviderSelector<TPlugin> Add<TPlugin>(string name = null);
		IConventionExpression Scan { get; }
		IDefaultsExpression Default { get; }
	}
}