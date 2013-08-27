using System;
using System.Collections.Generic;
using System.Reflection;
using Maestro.Conventions;

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
		IConventionExpression Matching(IConventionFilter filter);

		void AddConcreteSubClassesOf<T>();
		void AddConcreteSubClassesOf(Type type);
		void ForConcreteClassesClosing(Type genericTypeDefinition);
		void ForDefaultImplementations();
		void Using(IConvention convention);
	}
}