using System;
using System.Collections.Generic;

namespace Maestro.Conventions
{
	internal class ConcreteClassesClosingConvention : IConvention
	{
		private readonly Type _genericTypeDefinition;

		public ConcreteClassesClosingConvention(Type genericTypeDefinition)
		{
			_genericTypeDefinition = genericTypeDefinition;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types)
			{
				Type genericType;
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out genericType)) continue;
				containerExpression.For(genericType).Use(type);
			}
		}
	}
}