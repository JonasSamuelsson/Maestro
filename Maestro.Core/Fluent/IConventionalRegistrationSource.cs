using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maestro.Fluent
{
	public interface IConventionalRegistrationSource
	{
		IConventionalRegistration Assemblies(params Assembly[] assemblies);
		IConventionalRegistration Assemblies(IEnumerable<Assembly> assemblies);
		IConventionalRegistration Assembly(Assembly assembly);
		IConventionalRegistration AssemblyContaining<T>();
		IConventionalRegistration AssemblyContaining(Type type);
		IConventionalRegistration AssemblyContainingTypeOf(object o);
		IConventionalRegistration Types(IEnumerable<Type> types);
	}
}