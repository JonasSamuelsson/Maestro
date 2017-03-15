using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteSubClassesConvention<T> : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<ConventionalTypeInstanceRegistrator<T>> _serviceRegistration;

		public ConcreteSubClassesConvention(Type baseType, Action<ConventionalTypeInstanceRegistrator<T>> serviceRegistration)
		{
			_baseType = baseType;
			_serviceRegistration = serviceRegistration;
		}

		public void Process(IEnumerable<Type> types, ContainerConfigurator containerConfigurator)
		{
			foreach (var type in types.Where(x => x.IsConcreteSubClassOf(_baseType)))
			{
				_serviceRegistration(new ConventionalTypeInstanceRegistrator<T>(containerConfigurator, _baseType, type));
			}
		}
	}
}