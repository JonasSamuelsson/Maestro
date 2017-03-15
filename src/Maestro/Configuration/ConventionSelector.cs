using System;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public class ConventionSelector
	{
		private readonly Scanner _scanner;

		internal ConventionSelector(Scanner scanner)
		{
			_scanner = scanner;
		}

		public Scanner ConcreteSubClassesOf<T>(Action<ConventionalTypeInstanceRegistrator<T>> serviceRegistration)
		{
			return _scanner.With(new ConcreteSubClassesConvention<T>(typeof(T), serviceRegistration));
		}

		public Scanner ConcreteSubClassesOf(Type type, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration)
		{
			return _scanner.With(new ConcreteSubClassesConvention<object>(type, serviceRegistration));
		}

		public Scanner ConcreteClassesClosing(Type genericTypeDefinition, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration)
		{
			return _scanner.With(new ConcreteClassesClosingConvention(genericTypeDefinition, serviceRegistration));
		}

		public Scanner DefaultImplementations(Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration)
		{
			return _scanner.With(new DefaultImplementationsConvention(serviceRegistration));
		}
	}
}