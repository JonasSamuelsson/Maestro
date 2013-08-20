using System;

namespace Maestro.Fluent
{
	public interface IConventionalRegistration : IConventionalRegistrationSource
	{
		void AddConcreteSubClassesOf<T>();
		void AddConcreteSubClassesOf(Type type);
		void AddConcreteClassesClosing(Type genericTypeDefinition);
		IConventionalRegistration Matching(Func<Type, bool> predicate);
		IConventionalRegistration Matching(IConventionalRegistrationFilter filter);
		void UseDefaultImplementations();
		void Using(IConventionalRegistrator registrator);
	}
}