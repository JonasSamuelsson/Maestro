using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IConvention
	{
		void Process(IEnumerable<Type> types, IContainerConfiguration containerConfiguration);
	}
}