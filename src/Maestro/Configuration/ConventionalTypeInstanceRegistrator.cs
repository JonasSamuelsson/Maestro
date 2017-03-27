using System;

namespace Maestro.Configuration
{
	public class ConventionalTypeInstanceRegistrator<T>
	{
		private readonly IContainerExpression _containerExpression;
		private readonly Type _serviceType;
		private readonly Type _instanceType;

		public ConventionalTypeInstanceRegistrator(IContainerExpression containerExpression, Type serviceType, Type instanceType)
		{
			_containerExpression = containerExpression;
			_serviceType = serviceType;
			_instanceType = instanceType;
		}

		public ITypeInstanceExpression<T> Use(string name = null)
		{
			return name == null
				? _containerExpression.For(_serviceType).Use.Type(_instanceType).As<T>()
				: _containerExpression.For(_serviceType, name).Use.Type(_instanceType).As<T>();
		}

		public ITypeInstanceExpression<T> TryUse(string name = null)
		{
			return name == null
				? _containerExpression.For(_serviceType).TryUse.Type(_instanceType).As<T>()
				: _containerExpression.For(_serviceType, name).TryUse.Type(_instanceType).As<T>();
		}

		public ITypeInstanceExpression<T> Add()
		{
			return _containerExpression.For(_serviceType).Add.Type(_instanceType).As<T>();
		}
	}
}