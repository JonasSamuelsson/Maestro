using System;
using System.Collections.Generic;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class AddConcreteClassesClosingConvention : IConvention
	{
		private readonly Type _genericTypeDefinition;
		private readonly Action<ITypeInstanceExpression<object>> _configureAction;

		public AddConcreteClassesClosingConvention(Type genericTypeDefinition, Action<ITypeInstanceExpression<object>> configureAction)
		{
			_genericTypeDefinition = genericTypeDefinition;
			_configureAction = configureAction;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types)
			{
				Type genericType;
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out genericType)) continue;
				var typeInstanceExpression = containerExpression.For(genericType).Add(type);
				_configureAction(typeInstanceExpression);
			}
		}
	}
}