using System;
using System.Collections.Generic;
using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class ContainerExpression : IContainerExpression, IDisposable
	{
		private bool _disposed = false;
		private readonly Kernel _kernel;
		private readonly DefaultSettings _defaultSettings;

		public ContainerExpression(Kernel kernel, DefaultSettings defaultSettings)
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

		public IServiceExpression For(Type type)
		{
			if (type == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceNames.Default, _kernel, _defaultSettings);
		}

		public INamedServiceExpression For(Type type, string name)
		{
			if (type == null) throw new ArgumentNullException();
			if (name == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<object>(type, name, _kernel, _defaultSettings);
		}

		public IServiceExpression<T> For<T>()
		{
			AssertNotDisposed();
			return new ServiceExpression<T>(typeof(T), ServiceNames.Default, _kernel, _defaultSettings);
		}

		public INamedServiceExpression<T> For<T>(string name)
		{
			if (name == null) throw new ArgumentNullException();
			AssertNotDisposed();
			return new ServiceExpression<T>(typeof(T), name, _kernel, _defaultSettings);
		}

		public void Scan(Action<ScanExpression> scan)
		{
			AssertNotDisposed();
			scan(new ScanExpression(this, _defaultSettings));
		}

		private void AssertNotDisposed()
		{
			if (_disposed == false) return;
			throw new ObjectDisposedException(nameof(IContainerExpression));
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}
}