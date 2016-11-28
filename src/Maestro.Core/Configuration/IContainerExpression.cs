using System;

namespace Maestro.Configuration
{
	public interface IContainerExpression
	{
		IServiceExpression Service(Type type);
		IServiceExpression<T> Service<T>();

		IServiceExpression Service(Type type, string name);
		IServiceExpression<T> Service<T>(string name);

		IServicesExpression Services(Type type);
		IServicesExpression<T> Services<T>();

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