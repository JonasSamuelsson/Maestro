using System;

namespace Maestro.Fluent
{
	public interface IConventionalRegistration : IConventionalRegistrationSource
	{
		void AddConcreteSubClassesOf<T>();
		void AddConcreteSubClassesOf(Type type);
		void AddTypesClosing(Type genericTypeDefinition);
		IConventionalRegistration Matching(Func<Type, bool> predicate);
		IConventionalRegistration Matching(IConventionalRegistrationFilter filter);
		void Using(IConventionalRegistrator registrator);
	}
}