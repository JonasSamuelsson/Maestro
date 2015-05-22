using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Internals;

namespace Maestro.FactoryProviders.Factories
{
	class TypeFactory : IFactory
	{
		private readonly Type _typeToInstantiate;
		private readonly IEnumerable<Type> _constructorParameterTypes;

		public TypeFactory(Type typeToInstantiate, IEnumerable<Type> constructorParameterTypes)
		{
			_typeToInstantiate = typeToInstantiate;
			_constructorParameterTypes = constructorParameterTypes.ToList();
		}

		public object GetInstance(Context context)
		{
			var dependencies = _constructorParameterTypes.Select(t => context.Kernel.GetDependency(t, context)).ToArray();
			return Activator.CreateInstance(_typeToInstantiate, dependencies);
		}
	}
}