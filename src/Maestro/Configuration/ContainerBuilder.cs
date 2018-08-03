using Maestro.Internals;
using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public class ContainerBuilder
	{
		private readonly Container _container;
		private bool _disposed = false;

		internal ContainerBuilder(Container container)
		{
			_container = container;
		}

		public List<Func<Type, bool>> AutoResolveFilters => _container.Kernel.AutoResolveFilters;

		public IServiceBuilder Use(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceBuilder<object>(type, ServiceNames.Default, _container.Kernel, true);
		}

		public IServiceBuilder Use(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceBuilder<object>(type, name, _container.Kernel, true);
		}

		public IServiceBuilder<TService> Use<TService>()
		{
			AssertNotDisposed();
			return new ServiceBuilder<TService>(typeof(TService), ServiceNames.Default, _container.Kernel, true);
		}

		public IServiceBuilder<TService> Use<TService>(string name)
		{
			AssertNotDisposed();
			return new ServiceBuilder<TService>(typeof(TService), name, _container.Kernel, true);
		}

		public IServiceBuilder TryUse(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceBuilder<object>(type, ServiceNames.Default, _container.Kernel, false);
		}

		public IServiceBuilder TryUse(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceBuilder<object>(type, name, _container.Kernel, false);
		}

		public IServiceBuilder<TService> TryUse<TService>()
		{
			AssertNotDisposed();
			return new ServiceBuilder<TService>(typeof(TService), ServiceNames.Default, _container.Kernel, false);
		}

		public IServiceBuilder<TService> TryUse<TService>(string name)
		{
			AssertNotDisposed();
			return new ServiceBuilder<TService>(typeof(TService), name, _container.Kernel, false);
		}

		public IServiceBuilder Add(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceBuilder<object>(type, ServiceNames.Anonymous, _container.Kernel, true);
		}

		public IServiceBuilder<TService> Add<TService>()
		{
			AssertNotDisposed();
			return new ServiceBuilder<TService>(typeof(TService), ServiceNames.Anonymous, _container.Kernel, true);
		}

		public void Scan(Action<Scanner> action)
		{
			AssertNotDisposed();
			var scanner = new Scanner();
			action(scanner);
			scanner.Execute(this);
		}

		private void AssertNotDisposed()
		{
			if (_disposed == false) return;
			throw new InvalidOperationException($"{GetType().Name} can't be used outside of config closure.");
		}

		internal void Dispose()
		{
			_disposed = true;
		}
	}
}