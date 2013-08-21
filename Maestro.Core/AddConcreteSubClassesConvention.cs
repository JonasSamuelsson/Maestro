using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class AddConcreteSubClassesConvention : IConvention
	{
		private readonly Type _baseType;

		public AddConcreteSubClassesConvention(Type baseType)
		{
			_baseType = baseType;
		}

		public void Process(IEnumerable<Type> types, IContainerConfiguration containerConfiguration)
		{
			foreach (var type in types.Where(x => x.IsConcreteSubClassOf(_baseType)))
				containerConfiguration.Add(_baseType).Use(type);
		}
	}
}