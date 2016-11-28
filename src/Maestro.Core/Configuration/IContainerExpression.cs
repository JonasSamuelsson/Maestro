using System;

namespace Maestro.Configuration
{
	public interface IContainerExpression
	{
		IDefaultPluginExpression For(Type type);
		IDefaultPluginExpression<T> For<T>();

		IPluginExpression For(Type type, string name);
		IPluginExpression<T> For<T>(string name);

		/// <summary>
		/// Used for conventional configuration.
		/// </summary>
		IConventionExpression Scan { get; }

		/// <summary>
		/// Used to setup default convention filters, lifetimes etc.
		/// </summary>
		IDefaultSettingsExpression Default { get; }
	}
}