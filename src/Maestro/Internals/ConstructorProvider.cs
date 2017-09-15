using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Internals
{
	public class ConstructorProvider
	{
		public IEnumerable<ConstructorInfo> GetConstructors(Type type)
		{
			return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => !x.IsStatic)
				.Where(x => x.IsAssembly || x.IsPublic)
				.OrderByDescending(x => x.GetParameters().Length);
		}
	}
}