using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IInstanceExpression<object> For(Type type);
		IInstanceExpression<object> For(Type type, string name);
		IInstanceExpression<TPlugin> For<TPlugin>();
		IInstanceExpression<TPlugin> For<TPlugin>(string name);
		IInstanceExpression<object> Add(Type type);
		IInstanceExpression<TPlugin> Add<TPlugin>();
		IConventionExpression Scan { get; }
		IDefaultSettingsExpression Default { get; }
	}
}