using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface ITypeStack : IEnumerable<Type>
	{
		Type Root { get; }
		Type Current { get; }
	}
}