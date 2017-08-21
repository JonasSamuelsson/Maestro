﻿using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteClassesOfConvention<T> : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<IConventionalServiceTypeExpression<T>> _action;

		public ConcreteClassesOfConvention(Type baseType, Action<IConventionalServiceTypeExpression<T>> action)
		{
			_baseType = baseType;
			_action = action;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			Type genericType = null;
			foreach (var type in types.Where(x => x.IsConcreteClassOf(_baseType, out genericType)))
			{
				_action(new ConventionalServiceTypeExpression<T>(containerExpression, genericType ?? _baseType, type));
			}
		}
	}
}