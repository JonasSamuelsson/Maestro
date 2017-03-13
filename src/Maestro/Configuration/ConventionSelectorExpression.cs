using System;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public class ConventionSelectorExpression
	{
		private readonly ScanExpression _scanExpression;

		internal ConventionSelectorExpression(ScanExpression scanExpression)
		{
			_scanExpression = scanExpression;
		}

		public ScanExpression ConcreteSubClassesOf<T>(Action<TypeInstanceRegistrationExpression<T>> serviceRegistration)
		{
			return _scanExpression.With(new ConcreteSubClassesConvention<T>(typeof(T), serviceRegistration));
		}

		public ScanExpression ConcreteSubClassesOf(Type type, Action<TypeInstanceRegistrationExpression<object>> serviceRegistration)
		{
			return _scanExpression.With(new ConcreteSubClassesConvention<object>(type, serviceRegistration));
		}

		public ScanExpression ConcreteClassesClosing(Type genericTypeDefinition, Action<TypeInstanceRegistrationExpression<object>> serviceRegistration)
		{
			return _scanExpression.With(new ConcreteClassesClosingConvention(genericTypeDefinition, serviceRegistration));
		}

		public ScanExpression DefaultImplementations(Action<TypeInstanceRegistrationExpression<object>> serviceRegistration)
		{
			return _scanExpression.With(new DefaultImplementationsConvention(serviceRegistration));
		}
	}
}