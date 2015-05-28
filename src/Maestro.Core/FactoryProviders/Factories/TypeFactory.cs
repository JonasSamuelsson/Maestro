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
		private readonly Dictionary<Type, object> _constructorParameters;


		public TypeFactory(Type typeToInstantiate, IEnumerable<Type> constructorParameterTypes, Dictionary<Type, object> constructorParameters)
		{
			_typeToInstantiate = typeToInstantiate;
			_constructorParameterTypes = constructorParameterTypes.ToList();
			_constructorParameters = constructorParameters;
		}

		public object GetInstance(Context context)
		{
			object value;
			var parameters = from type in _constructorParameterTypes
								  select _constructorParameters.TryGetValue(type, out value)
												? value
												: context.Kernel.GetDependency(type, context);
			return Activator.CreateInstance(_typeToInstantiate, parameters.ToArray());
		}
	}
}