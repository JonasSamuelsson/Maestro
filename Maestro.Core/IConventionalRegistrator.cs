using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface IConventionalRegistrator
	{
		void Process(IEnumerable<Type> types, IContainerConfiguration containerConfiguration);
	}
}