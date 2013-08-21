using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maestro.Fluent
{
	public interface IConventionExpression
	{
		IConventionExpression Assemblies(params Assembly[] assemblies);
		IConventionExpression Assemblies(IEnumerable<Assembly> assemblies);
		IConventionExpression Assembly(Assembly assembly);
		IConventionExpression AssemblyContaining<T>();
		IConventionExpression AssemblyContaining(Type type);
		IConventionExpression AssemblyContainingTypeOf(object o);
		IConventionExpression Types(IEnumerable<Type> types);

		IConventionExpression Where(Func<Type, bool> predicate);
		IConventionExpression Matching(IConventionalRegistrationFilter filter);

		void AddConcreteSubClassesOf<T>();
		void AddConcreteSubClassesOf(Type type);
		void AddConcreteClassesClosing(Type genericTypeDefinition);
		void ForDefaultImplementations();
		void Using(IConvention registrator);
	}
}