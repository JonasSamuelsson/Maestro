using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class AddConcreteSubClassesConvention : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<IInstanceBuilderExpression<object>> _instanceConfiguration;

		public AddConcreteSubClassesConvention(Type baseType, Action<IInstanceBuilderExpression<object>> instanceConfiguration)
		{
			_baseType = baseType;
			_instanceConfiguration = instanceConfiguration;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types.Where(x => x.IsConcreteSubClassOf(_baseType)))
			{
				var instanceBuilderExpression = containerExpression.For(_baseType).Add(type);
				_instanceConfiguration(instanceBuilderExpression);
			}
		}
	}
}