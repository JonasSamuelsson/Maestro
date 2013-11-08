using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IInstanceFactoryExpression<object> For(Type type);
		IInstanceFactoryExpression<object> For(Type type, string name);
		IInstanceFactoryExpression<TPlugin> For<TPlugin>();
		IInstanceFactoryExpression<TPlugin> For<TPlugin>(string name);
		IInstanceFactoryExpression<object> Add(Type type);
		IInstanceFactoryExpression<TPlugin> Add<TPlugin>();
		IConventionExpression Scan { get; }
		IDefaultSettingsExpression Default { get; }
	}
}