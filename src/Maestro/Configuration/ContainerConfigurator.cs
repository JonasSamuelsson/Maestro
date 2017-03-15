using System;
using System.Collections.Generic;
using Maestro.Internals;

namespace Maestro.Configuration
{
	public class ContainerConfigurator : IDisposable
	{
		private bool _disposed = false;
		private readonly Kernel _kernel;
		private readonly DefaultSettings _defaultSettings;

		internal ContainerConfigurator(Kernel kernel, DefaultSettings defaultSettings)
		{
			_kernel = kernel;
			_defaultSettings = defaultSettings;
		}

		public IDefaultSettingsExpression Default
		{
			get
			{
				AssertNotDisposed();
				return _defaultSettings;
			}
		}

		public IList<ITypeProvider> TypeProviders => _kernel.TypeProviders;

		public IServiceConfigurator For(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceConfigurator<object>(type, ServiceNames.Default, _kernel, _defaultSettings);
		}

		public INamedServiceConfigurator For(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			if (name == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceConfigurator<object>(type, name, _kernel, _defaultSettings);
		}

		public IServiceConfigurator<T> For<T>()
		{
			AssertNotDisposed();
			return new ServiceConfigurator<T>(typeof(T), ServiceNames.Default, _kernel, _defaultSettings);
		}

		public INamedServiceConfigurator<T> For<T>(string name)
		{
			if (name == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceConfigurator<T>(typeof(T), name, _kernel, _defaultSettings);
		}

		public void Scan(Action<Scanner> action)
		{
			AssertNotDisposed();
			var scanner = new Scanner();
			action(scanner);
			scanner.Execute(this, _defaultSettings);
		}

		private void AssertNotDisposed()
		{
			if (_disposed == false) return;
			throw new ObjectDisposedException(nameof(ContainerConfigurator));
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}