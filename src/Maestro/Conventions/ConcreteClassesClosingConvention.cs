using Maestro.Configuration;
using System;
using System.Collections.Generic;

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
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out var genericTypes)) continue;
				genericTypes.ForEach(x => _action(new ConventionalServiceExpression<object>(containerExpression, x, type)));
			}
		}
	}
}