using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteSubClassesConvention<T> : IConvention
	{
		private readonly Type _baseType;
		private readonly ServiceRegistration _registration;
		private readonly Action<ITypeInstanceExpression<T>> _instanceConfiguration;

		public ConcreteSubClassesConvention(Type baseType, ServiceRegistration registration, Action<ITypeInstanceExpression<T>> instanceConfiguration)
		{
			_baseType = baseType;
			_registration = registration;
			_instanceConfiguration = instanceConfiguration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types.Where(x => x.IsConcreteSubClassOf(_baseType)))
			{
				var typeInstanceExpression = _registration.Invoke(_baseType, type);
				if (typeInstanceExpression == null) continue;
				_instanceConfiguration?.Invoke(typeInstanceExpression.As<T>());
			}
		}
	}
}