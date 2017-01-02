using System;
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

		public IConventionExpression Scan
		{
			get
			{
				AssertNotDisposed();
				return new ConventionExpression(this, _defaultSettings);
			}
		}

		public IDefaultSettingsExpression Default
		{
			get
			{
				AssertNotDisposed();
				return _defaultSettings;
			}
		}

		public IServiceExpression For(Type type)
		{
			AssertNotDisposed();
			return new ServiceExpression<object>(type, ServiceDescriptorLookup.DefaultName, _kernel, _defaultSettings);
		}

		public INamedServiceExpression For(Type type, string name)
		{
			AssertNotDisposed();
			return new ServiceExpression<object>(type, name ?? ServiceDescriptorLookup.DefaultName, _kernel, _defaultSettings);
		}

		public IServiceExpression<T> For<T>()
		{
			AssertNotDisposed();
			return new ServiceExpression<T>(typeof(T), ServiceDescriptorLookup.DefaultName, _kernel, _defaultSettings);
		}

		public INamedServiceExpression<T> For<T>(string name)
		{
			AssertNotDisposed();
			return new ServiceExpression<T>(typeof(T), name ?? ServiceDescriptorLookup.DefaultName, _kernel, _defaultSettings);
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