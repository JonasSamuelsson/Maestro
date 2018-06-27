using Maestro.Internals;
using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public class ContainerExpression
	{
		private readonly Container _container;
		private bool _disposed = false;

		internal ContainerExpression(Container container)
		{
			_container = container;
		}

		public List<Func<Type, bool>> AutoResolveFilters => _container.Kernel.AutoResolveFilters;

		public IServiceExpression Use(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Default, _container.Kernel, true);
		}

		public IServiceExpression Use(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, name, _container.Kernel, true);
		}

		public IServiceExpression<TService> Use<TService>()
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), ServiceNames.Default, _container.Kernel, true);
		}

		public IServiceExpression<TService> Use<TService>(string name)
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), name, _container.Kernel, true);
		}

		public IServiceExpression TryUse(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Default, _container.Kernel, false);
		}

		public IServiceExpression TryUse(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, name, _container.Kernel, false);
		}

		public IServiceExpression<TService> TryUse<TService>()
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), ServiceNames.Default, _container.Kernel, false);
		}

		public IServiceExpression<TService> TryUse<TService>(string name)
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), name, _container.Kernel, false);
		}

		public IServiceExpression Add(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Anonymous, _container.Kernel, true);
		}

		public IServiceExpression<TService> Add<TService>()
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), ServiceNames.Anonymous, _container.Kernel, true);
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