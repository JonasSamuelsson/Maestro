using Maestro.Internals;
using System;
using System.Collections.Generic;

namespace Maestro.Configuration
{
	public class ContainerExpression
	{
		private bool _disposed = false;
		private readonly Kernel _kernel;

		internal ContainerExpression(Kernel kernel)
		{
			_kernel = kernel;
		}

		public List<Func<Type, bool>> AutoResolveFilters => _kernel.AutoResolveFilters;

		public IServiceExpression Use(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Default, _kernel, true);
		}

		public IServiceExpression Use(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, name, _kernel, true);
		}

		public IServiceExpression<TService> Use<TService>()
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), ServiceNames.Default, _kernel, true);
		}

		public IServiceExpression<TService> Use<TService>(string name)
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), name, _kernel, true);
		}

		public IServiceExpression TryUse(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Default, _kernel, false);
		}

		public IServiceExpression TryUse(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, name, _kernel, false);
		}

		public IServiceExpression<TService> TryUse<TService>()
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), ServiceNames.Default, _kernel, false);
		}

		public IServiceExpression<TService> TryUse<TService>(string name)
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), name, _kernel, false);
		}

		public IServiceExpression Add(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Anonymous, _kernel, true);
		}

		public IServiceExpression<TService> Add<TService>()
		{
			AssertNotDisposed();
			return new ServiceExpression<TService>(typeof(TService), ServiceNames.Anonymous, _kernel, true);
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