using System;
using System.Collections.Generic;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	/// <summary>
	/// Base type for conventional registrations.
	/// </summary>
	public interface IConvention
	{
		void Process(IEnumerable<Type> types, ContainerConfigurator containerConfigurator);
	}
}