using System;

namespace Maestro.Configuration
{
	public interface IConventionSelectorExpression
	{
		IConventionExpression ConcreteSubClassesOf<T>(Action<ITypeInstanceExpression<T>> instanceConfiguration = null);
		IConventionExpression ConcreteSubClassesOf(Type type, Action<ITypeInstanceExpression<object>> instanceConfiguration = null);
		IConventionExpression ConcreteClassesClosing(Type genericTypeDefinition, Action<ITypeInstanceExpression<object>> instanceConfiguration = null);
		IConventionExpression DefaultImplementations(Action<ITypeInstanceExpression<object>> instanceConfiguration = null);
	}
}