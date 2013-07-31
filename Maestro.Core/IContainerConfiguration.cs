using Maestro.Fluent;
using System;

namespace Maestro
{
	public interface IContainerConfiguration
	{
		IPipelineSelector Default(Type type);
	}
}