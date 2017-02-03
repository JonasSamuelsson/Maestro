using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public interface IContainerExpression
	{
		IServiceExpression For(Type type);
		IServiceExpression<T> For<T>();

		INamedServiceExpression For(Type type, string name);
		INamedServiceExpression<T> For<T>(string name);

		/// <summary>
		/// Used for conventional configuration.
		/// </summary>
		IConventionExpression Scan { get; }

		/// <summary>
		/// Used to setup default convention filters, lifetimes etc.
		/// </summary>
		IDefaultSettingsExpression Default { get; }

		IList<ITypeProvider> TypeProviders { get; }
	}
}