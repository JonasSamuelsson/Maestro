using System;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	internal class ConventionSelector : IConventionSelector
	{
		private readonly IScanner _scanner;

		internal ConventionSelector(IScanner scanner)
		{
			_scanner = scanner;
		}

		public IScanner ConcreteSubClassesOf<T>(Action<ConventionalTypeInstanceRegistrator<T>> serviceRegistration)
		{
			return _scanner.With(new ConcreteSubClassesConvention<T>(typeof(T), serviceRegistration));
		}

		public IScanner ConcreteSubClassesOf(Type type, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration)
		{
			return _scanner.With(new ConcreteSubClassesConvention<object>(type, serviceRegistration));
		}

		public IScanner ConcreteClassesClosing(Type genericTypeDefinition, Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration)
		{
			return _scanner.With(new ConcreteClassesClosingConvention(genericTypeDefinition, serviceRegistration));
		}

		public IScanner DefaultImplementations(Action<ConventionalTypeInstanceRegistrator<object>> serviceRegistration)
		{
			return _scanner.With(new DefaultImplementationsConvention(serviceRegistration));
		}
	}
}