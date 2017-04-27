using System;

namespace Maestro.Configuration
{
	public class ConventionalServiceTypeSelector<T> : IConventionalServiceTypeSelector<T>
	{
		private readonly IContainerExpression _containerExpression;
		private readonly Type _baseType;
		private readonly Type _type;

		public ConventionalServiceTypeSelector(IContainerExpression containerExpression, Type baseType, Type type)
		{
			_containerExpression = containerExpression;
			_baseType = baseType;
			_type = type;
		}

		public IConventionalTypeInstanceRegistrator<T> BaseType => new ConventionalTypeInstanceRegistrator<T>(_containerExpression, _baseType, _type);
		public IConventionalTypeInstanceRegistrator<T> Type => new ConventionalTypeInstanceRegistrator<T>(_containerExpression, _type, _type);
	}
}