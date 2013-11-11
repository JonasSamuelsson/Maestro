using System;
using Maestro.Configuration;

namespace Maestro
{
	public interface IContainerExpression
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