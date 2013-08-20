using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Fluent
{
	internal class ConcreteSubClassRegistrator : IConvention
	{
		private readonly Type _baseType;

		public ConcreteSubClassRegistrator(Type baseType)
		{
			_baseType = baseType;
		}

		public void Process(IEnumerable<Type> types, IContainerConfiguration containerConfiguration)
		{
			foreach (var type in types.Where(x => TypeHelper.IsConcreteSubClassOf(x, _baseType)))
				containerConfiguration.Add(_baseType).Use(type);
		}
	}
}