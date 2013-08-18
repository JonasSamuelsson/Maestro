using System;

namespace Maestro.Fluent
{
	public interface IConventionalRegistration : IConventionalRegistrationSource
	{
		ITypeInstancePipelineBuilder<T> AddSubTypesOf<T>();
		ITypeInstancePipelineBuilder<object> AddSubTypesOf(Type type);
		ITypeInstancePipelineBuilder<object> AddTypesClosing(Type genericTypeDefinition);
		IConventionalRegistration Matching(Func<Type, bool> predicate);
		IConventionalRegistration Matching(IConventionalRegistrationFilter filter);
		void Using(IConventionalRegistrator registrator);
	}
}