using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IProviderSelector<object> For(Type type);
		IProviderSelector<object> For(Type type, string name);
		IProviderSelector<TPlugin> For<TPlugin>();
		IProviderSelector<TPlugin> For<TPlugin>(string name);
		IProviderSelector<object> Add(Type type);
		IProviderSelector<TPlugin> Add<TPlugin>();
		IConventionExpression Scan { get; }
		IDefaultSettingsExpression Default { get; }
	}
}