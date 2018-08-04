using System;

namespace Maestro.Configuration
{
	internal class ConventionalServiceBuilder<T> : IConventionalServiceBuilder<T>
	{
		private readonly ContainerBuilder _containerBuilder;
		private readonly Type _serviceType;
		private readonly Type _instanceType;

		public ConventionalServiceBuilder(ContainerBuilder containerBuilder, Type serviceType, Type instanceType)
		{
			_containerBuilder = containerBuilder;
			_serviceType = serviceType;
			_instanceType = instanceType;
		}

		public ITypeInstanceBuilder<T> Use()
		{
			return _containerBuilder.Add(_serviceType).Type(_instanceType).As<T>();
		}

		public ITypeInstanceBuilder<T> Use(string name)
		{
			return _containerBuilder.Add(_serviceType).Named(name).Type(_instanceType).As<T>();
		}

		public ITypeInstanceBuilder<T> TryUse()
		{
			return _containerBuilder.TryAdd(_serviceType).Type(_instanceType).As<T>();
		}

		public ITypeInstanceBuilder<T> TryUse(string name)
		{
			return _containerBuilder.TryAdd(_serviceType)?.Named(name).Type(_instanceType).As<T>();
		}

		public ITypeInstanceBuilder<T> Add()
		{
			return _containerBuilder.Add(_serviceType).Type(_instanceType).As<T>();
		}
	}
}