using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IProviderSelector<object> For(Type type);
		IProviderSelector<TPlugin> For<TPlugin>();
		IProviderSelector<object> Add(Type type, string name = null);
		IProviderSelector<TPlugin> Add<TPlugin>(string name = null);
		IConventionExpression Scan { get; }
		IDefaultSettingsExpression Default { get; }
	}
}