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
			return ConstructorResolver.TryGetConstructor(_type, context, out constructor);
		}

		public object Get(IContext context)
		{
			ConstructorInfo constructor;
			if (!ConstructorResolver.TryGetConstructor(_type, context, out constructor))
				throw new InvalidOperationException(string.Format("Can't find appropriate constructor to invoke {0}.", _type.FullName));
			var ctorArgs = constructor.GetParameters().Select(x => context.Get(x.ParameterType)).ToArray();
			return Activator.CreateInstance(_type, ctorArgs);
		}
	}
}