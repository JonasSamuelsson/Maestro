using System;

namespace Maestro.Configuration
{
	public class ConventionalTypeInstanceRegistrator<T>
	{
		private readonly ContainerConfigurator _containerConfigurator;
		private readonly Type _serviceType;
		private readonly Type _instanceType;

		public ConventionalTypeInstanceRegistrator(ContainerConfigurator containerConfigurator, Type serviceType, Type instanceType)
		{
			_containerConfigurator = containerConfigurator;
			_serviceType = serviceType;
			_instanceType = instanceType;
		}

		public ITypeInstanceConfigurator<T> Use(string name = null)
		{
			return name == null
				? _containerConfigurator.For(_serviceType).Use.Type(_instanceType).As<T>()
				: _containerConfigurator.For(_serviceType, name).Use.Type(_instanceType).As<T>();
		}

		public ITypeInstanceConfigurator<T> TryUse(string name = null)
		{
			return name == null
				? _containerConfigurator.For(_serviceType).TryUse.Type(_instanceType).As<T>()
				: _containerConfigurator.For(_serviceType, name).TryUse.Type(_instanceType).As<T>();
		}

		public ITypeInstanceConfigurator<T> Add()
		{
			return _containerConfigurator.For(_serviceType).Add.Type(_instanceType).As<T>();
		}
	}
}