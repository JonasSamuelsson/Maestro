using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteSubClassesConvention<T> : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<TypeInstanceRegistrationExpression<T>> _serviceRegistration;

		public ConcreteSubClassesConvention(Type baseType, Action<TypeInstanceRegistrationExpression<T>> serviceRegistration)
		{
			_baseType = baseType;
			_serviceRegistration = serviceRegistration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types.Where(x => x.IsConcreteSubClassOf(_baseType)))
			{
				_serviceRegistration(new TypeInstanceRegistrationExpression<T>(containerExpression, _baseType, type));
			}
		}
	}
}