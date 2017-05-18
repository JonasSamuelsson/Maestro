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

		public IScanner ConcreteClassesOf<T>(Action<IConventionalServiceTypeSelector<T>> serviceRegistration)
		{
			return _scanner.Using(new ConcreteClassesOfConvention<T>(typeof(T), serviceRegistration));
		}

		public IScanner ConcreteClassesOf(Type type, Action<IConventionalServiceTypeSelector<object>> serviceRegistration)
		{
			return _scanner.Using(new ConcreteClassesOfConvention<object>(type, serviceRegistration));
		}

		public IScanner ConcreteClassesClosing(Type genericTypeDefinition, Action<IConventionalServiceTypeSelector<object>> serviceRegistration)
		{
			return _scanner.Using(new ConcreteClassesClosingConvention(genericTypeDefinition, serviceRegistration));
		}

		public IScanner DefaultImplementations(Action<IConventionalServiceTypeSelector<object>> serviceRegistration)
		{
			return _scanner.Using(new DefaultImplementationsConvention(serviceRegistration));
		}
	}
}