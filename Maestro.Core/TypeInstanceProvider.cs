using System;
using System.Linq;
using System.Reflection;

namespace Maestro
{
	internal class TypeInstanceProvider : IProvider
	{
		private readonly Type _type;

		public TypeInstanceProvider(Type type)
		{
			_type = type;
		}

		public bool CanGet(IContext context)
		{
			ConstructorInfo constructor;
			return TryGetConstructor(_type, context, out constructor);
		}

		public object Get(IContext context)
		{
			ConstructorInfo constructor;
			if (!TryGetConstructor(_type, context, out constructor))
				throw new ActivationException();
			var ctorArgs = constructor.GetParameters().Select(x => context.Get(x.ParameterType)).ToArray();
			return Activator.CreateInstance(_type, ctorArgs);
		}

		private static bool TryGetConstructor(Type type, IContext context, out ConstructorInfo constructor)
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