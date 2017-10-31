using System;

namespace Maestro.Configuration
{
	internal class ConventionalServiceExpression<T> : IConventionalServiceExpression<T>
	{
		private readonly IContainerExpression _containerExpression;
		private readonly Type _serviceType;
		private readonly Type _instanceType;

		public ConventionalServiceExpression(IContainerExpression containerExpression, Type serviceType, Type instanceType)
		{
			_containerExpression = containerExpression;
			_serviceType = serviceType;
			_instanceType = instanceType;
		}

		public ITypeInstanceExpression<T> Use()
		{
			return _containerExpression.Use(_serviceType).Type(_instanceType).As<T>();
		}

		public ITypeInstanceExpression<T> Use(string name)
		{
			return _containerExpression.Use(_serviceType, name).Type(_instanceType).As<T>();
		}

		public ITypeInstanceExpression<T> TryUse()
		{
			return _containerExpression.TryUse(_serviceType).Type(_instanceType).As<T>();
		}

		public ITypeInstanceExpression<T> TryUse(string name)
		{
			return _containerExpression.TryUse(_serviceType, name).Type(_instanceType).As<T>();
		}

		public ITypeInstanceExpression<T> Add()
		{
			return _containerExpression.Add(_serviceType).Type(_instanceType).As<T>();
		}
	}
}