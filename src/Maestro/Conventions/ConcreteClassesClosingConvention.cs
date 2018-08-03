using Maestro.Configuration;
using System;
using System.Collections.Generic;

namespace Maestro.Conventions
{
	internal class ConcreteClassesClosingConvention : IConvention
	{
		private readonly Type _genericTypeDefinition;
		private readonly Action<IConventionalServiceBuilder<object>> _action;

		public ConcreteClassesClosingConvention(Type genericTypeDefinition, Action<IConventionalServiceBuilder<object>> action)
		{
			_genericTypeDefinition = genericTypeDefinition;
			_action = action;
		}

		public void Process(IEnumerable<Type> types, ContainerBuilder containerBuilder)
		{
			foreach (var type in types)
			{
				if (!type.IsConcreteClassClosing(_genericTypeDefinition, out var genericTypes)) continue;
				genericTypes.ForEach(x => _action(new ConventionalServiceBuilder<object>(containerBuilder, x, type)));
			}
		}
	}
}