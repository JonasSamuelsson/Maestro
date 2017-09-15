using System;
using System.Linq;
using System.Reflection;

namespace Maestro.Internals
{
	internal class PropertyProvider
	{
		public PropertyInfo GetProperty(Type type, string name)
		{
			return type
				.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => x.CanWrite)
				.Where(x => x.SetMethod.IsAssembly || x.SetMethod.IsPublic)
				.SingleOrDefault(x => x.Name == name);
		}
	}
}