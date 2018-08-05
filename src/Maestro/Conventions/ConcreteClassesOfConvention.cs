using Maestro.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Conventions
{
	internal class ConcreteClassesOfConvention<T> : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<IConventionalServiceRegistrator<T>> _action;

		public ConcreteClassesOfConvention(Type baseType, Action<IConventionalServiceRegistrator<T>> action)
		{
			_baseType = baseType;
			_action = action;
		}

		public void Process(IEnumerable<Type> types, ContainerBuilder containerBuilder)
		{
			Type genericType = null;
			foreach (var type in types.Where(x => x.IsConcreteClassOf(_baseType, out genericType)))
			{
				new ConventionalServiceBuilder<T>(containerBuilder, genericType ?? _baseType, type)
					.Execute(_action);
			}
		}
	}
}