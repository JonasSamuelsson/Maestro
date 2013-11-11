using System;
using System.Collections.Generic;

namespace Maestro.Conventions
{
	public interface IConvention
	{
		void Process(IEnumerable<Type> types, IContainerExpression containerExpression);
	}
}