using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;

namespace Maestro.Conventions
{
	internal class AddConcreteSubClassesConvention : IConvention
	{
		private readonly Type _baseType;
		private readonly Action<ITypeInstanceExpression<object>> _configureAction;

		public AddConcreteSubClassesConvention(Type baseType, Action<ITypeInstanceExpression<object>> configureAction)
		{
			_baseType = baseType;
			_configureAction = configureAction;
		}

		public void Process(IEnumerable<Type> types, IContainerExpression containerExpression)
		{
			foreach (var type in types.Where(x => x.IsConcreteSubClassOf(_baseType)))
			{
				var typeInstanceExpression = containerExpression.Services(_baseType).Add.Type(type);
				_configureAction(typeInstanceExpression);
			}
		}
	}
}