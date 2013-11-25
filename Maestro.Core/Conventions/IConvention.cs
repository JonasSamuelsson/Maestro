using System;
using System.Collections.Generic;

namespace Maestro.Conventions
{
	/// <summary>
	/// Base type for conventional registrations.
	/// </summary>
	public interface IConvention
	{
		void Process(IEnumerable<Type> types, IContainerExpression containerExpression);
	}
}