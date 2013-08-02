using System;
using System.Linq;
using System.Reflection;

namespace Maestro
{
	internal static class ConstructorResolver
	{
		public static bool TryGetConstructor(Type type, IContext context, out ConstructorInfo constructor)
		{
			constructor = null;
			foreach (var ctor in type.GetConstructors().OrderByDescending(x => x.GetParameters().Length))
			{
				var parameterTypes = ctor.GetParameters().Select(x => x.ParameterType).ToList();
				if (parameterTypes.Any() && !parameterTypes.All(context.CanGet)) continue;
				constructor = ctor;
				return true;
			}
			return false;
		}
	}
}