﻿using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteClassesOfConvention<T> : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<ConventionalTypeInstanceRegistrator<T>> _serviceRegistration;

		public ConcreteClassesOfConvention(Type baseType, Action<ConventionalTypeInstanceRegistrator<T>> serviceRegistration)
		{
			_baseType = baseType;
			_serviceRegistration = serviceRegistration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
		   Type genericType = null;
			foreach (var type in types.Where(x => x.IsConcreteClassOf(_baseType, out genericType)))
			{
				_serviceRegistration(new ConventionalTypeInstanceRegistrator<T>(containerExpression, genericType ?? _baseType, type));
			}
		}
	}
}