using System;
using System.Collections.Generic;

namespace Maestro
{
	public interface ITypeStack : IEnumerable<Type>
	{
		Type RootType { get; }
		Type CurrentType { get; }
	}
}