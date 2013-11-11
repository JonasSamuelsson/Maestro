﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Conventions
{
	internal class AddConcreteSubClassesConvention : IConvention
	{
		private readonly Type _baseType;

		public AddConcreteSubClassesConvention(Type baseType)
		{
			_baseType = baseType;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types.Where(x => x.IsConcreteSubClassOf(_baseType)))
				containerExpression.Add(_baseType).Use(type);
		}
	}
}