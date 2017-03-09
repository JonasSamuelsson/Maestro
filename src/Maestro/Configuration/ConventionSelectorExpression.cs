using System;
using Maestro.Conventions;

namespace Maestro.Configuration
{
	public class ConventionSelectorExpression
	{
		private readonly IScanExpression _scanExpression;

		internal ConventionSelectorExpression(IScanExpression scanExpression)
		{
			_scanExpression = scanExpression;
		}

		public IScanExpression ConcreteSubClassesOf<T>(Action<TypeInstanceRegistrationExpression<T>> serviceRegistration)
		{
			return _scanExpression.With(new ConcreteSubClassesConvention<T>(typeof(T), serviceRegistration));
		}

		public IScanExpression ConcreteSubClassesOf(Type type, Action<TypeInstanceRegistrationExpression<object>> serviceRegistration)
		{
			return _scanExpression.With(new ConcreteSubClassesConvention<object>(type, serviceRegistration));
		}

		public IScanExpression ConcreteClassesClosing(Type genericTypeDefinition, Action<TypeInstanceRegistrationExpression<object>> serviceRegistration)
		{
			return _scanExpression.With(new ConcreteClassesClosingConvention(genericTypeDefinition, serviceRegistration));
		}

		public IScanExpression DefaultImplementations(Action<TypeInstanceRegistrationExpression<object>> serviceRegistration)
		{
			return _scanExpression.With(new DefaultImplementationsConvention(serviceRegistration));
		}
	}
}