using System;
using System.Collections.Generic;
using System.Reflection;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public interface IScanner
	{
		IScanner Assemblies(IEnumerable<Assembly> assemblies);
		IScanner Assembly(Assembly assembly);
		IScanner AssemblyContaining<T>();
		IScanner AssemblyContaining(Type type);
		IScanner AssemblyContainingTypeOf(object o);
		IScanner Types(IEnumerable<Type> types);
		IScanner Matching(Func<Type, bool> predicate);
		IScanner Matching<T>() where T : IFilter, new();
		IScanner Matching(IFilter filter);
		IScanner With<T>() where T : IConvention, new();
		IScanner With(IConvention convention);
	}
}