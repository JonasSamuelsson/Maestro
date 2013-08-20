using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IPipelineSelector For(Type type);
		IPipelineSelector<TPlugin> For<TPlugin>();
		IPipelineSelector Add(Type type, string name = null);
		IPipelineSelector<TPlugin> Add<TPlugin>(string name = null);
		IConventionExpression Scan { get; }
	}
}