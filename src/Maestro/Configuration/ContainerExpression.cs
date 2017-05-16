using System;
using System.Collections.Generic;
using Maestro.Internals;

namespace Maestro.Configuration
{
   internal class ContainerExpression : IDisposable, IContainerExpression
	{
		private bool _disposed = false;
		private readonly Kernel _kernel;

		internal ContainerExpression(Kernel kernel)
		{
			_kernel = kernel;
		}

		public IDefaultsExpression Settings
		{
			get
			{
				AssertNotDisposed();
				return new DefaultsExpression(_kernel.DefaultSettings);
			}
		}

		public IList<ITypeProvider> TypeProviders => _kernel.TypeProviders;

		public IServiceExpression For(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Default, _kernel);
		}

		public INamedServiceExpression For(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			if (name == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, name, _kernel);
		}

		public IServiceExpression<T> For<T>()
		{
			AssertNotDisposed();
			return new ServiceExpression<T>(typeof(T), ServiceNames.Default, _kernel);
		}

		public INamedServiceExpression<T> For<T>(string name)
		{
			if (name == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<T>(typeof(T), name, _kernel);
		}

		public void Scan(Action<IScanner> action)
		{
			AssertNotDisposed();
			var scanner = new Scanner();
			action(scanner);
			scanner.Execute(this);
		}

		private void AssertNotDisposed()
		{
			if (_disposed == false) return;
			throw new ObjectDisposedException(nameof(ContainerExpression));
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}