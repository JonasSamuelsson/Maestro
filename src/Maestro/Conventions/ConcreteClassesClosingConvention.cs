﻿using System;
using System.Collections.Generic;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteClassesClosingConvention : IConvention
	{
		private readonly Type _genericTypeDefinition;
		private readonly Action<IConventionalServiceExpression<object>> _action;

		public ConcreteClassesClosingConvention(Type genericTypeDefinition, Action<IConventionalServiceExpression<object>> action)
		{
			_genericTypeDefinition = genericTypeDefinition;
			_action = action;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types)
			{
				Type genericType;
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out genericType)) continue;
				_action(new ConventionalServiceExpression<object>(containerExpression, genericType, type));
			}
		}
	}
}