using System;
using System.Collections.Generic;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteClassesClosingConvention : IConvention
	{
		private readonly Type _genericTypeDefinition;
		private readonly ServiceRegistration _registration;
		private readonly Action<ITypeInstanceExpression<object>> _instanceConfiguration;

		public ConcreteClassesClosingConvention(Type genericTypeDefinition, ServiceRegistration registration, Action<ITypeInstanceExpression<object>> instanceConfiguration)
		{
			_genericTypeDefinition = genericTypeDefinition;
			_registration = registration;
			_instanceConfiguration = instanceConfiguration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types)
			{
				Type genericType;
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out genericType)) continue;
				var typeInstanceExpression = _registration.Invoke(genericType, type);
				if (typeInstanceExpression == null) continue;
				_instanceConfiguration?.Invoke(typeInstanceExpression);
			}
		}
	}
}