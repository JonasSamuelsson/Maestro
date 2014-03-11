using Maestro.Configuration;
using System;
using System.Collections.Generic;

namespace Maestro.Conventions
{
	internal class AddConcreteClassesClosingConvention : IConvention
	{
		private readonly Type _genericTypeDefinition;
		private readonly Action<IInstanceBuilderExpression<object>> _instanceConfiguration;

		public AddConcreteClassesClosingConvention(Type genericTypeDefinition, Action<IInstanceBuilderExpression<object>> instanceConfiguration)
		{
			_genericTypeDefinition = genericTypeDefinition;
			_instanceConfiguration = instanceConfiguration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types)
			{
				Type genericType;
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out genericType)) continue;
				var instanceBuilderExpression = containerExpression.For(genericType).Add(type);
				_instanceConfiguration(instanceBuilderExpression);
			}
		}
	}
}