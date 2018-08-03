using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class ConcreteClassesOfConvention<T> : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<IConventionalServiceBuilder<T>> _action;

		public ConcreteClassesOfConvention(Type baseType, Action<IConventionalServiceBuilder<T>> action)
		{
			_baseType = baseType;
			_action = action;
		}

		public void Process(IEnumerable<Type> types, ContainerBuilder containerBuilder)
		{
			Type genericType = null;
			foreach (var type in types.Where(x => x.IsConcreteClassOf(_baseType, out genericType)))
			{
				_action(new ConventionalServiceBuilder<T>(containerBuilder, genericType ?? _baseType, type));
			}
		}
	}
}