using System;
using System.Collections.Generic;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteClassesClosingConvention : IConvention
	{
		private readonly Type _genericTypeDefinition;
		private readonly Action<IConventionalServiceTypeSelector<object>> _serviceRegistration;

		public ConcreteClassesClosingConvention(Type genericTypeDefinition, Action<IConventionalServiceTypeSelector<object>> serviceRegistration)
		{
			_genericTypeDefinition = genericTypeDefinition;
			_serviceRegistration = serviceRegistration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types)
			{
				Type genericType;
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out genericType)) continue;
				_serviceRegistration(new ConventionalServiceTypeSelector<object>(containerExpression, genericType, type));
			}
		}
	}
}