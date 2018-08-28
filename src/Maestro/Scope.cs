using Maestro.Internals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	public abstract class Scope : IScope
	{
		private IServiceProvider _serviceProvider;
		protected bool Disposed { get; private set; }

		internal Scope(Kernel kernel)
		{
			Kernel = kernel;
		}

		internal ConcurrentDictionary<object, object> Cache { get; } = new ConcurrentDictionary<object, object>();
		internal Kernel Kernel { get; }

		public Diagnostics.Diagnostics Diagnostics => new Diagnostics.Diagnostics(Kernel);

		public object GetService(Type type)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.GetService(type, ServiceNames.Default);
		}

		public object GetService(Type type, string name)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.GetService(type, name);
		}

		public T GetService<T>()
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.GetService<T>(ServiceNames.Default);
		}

		public T GetService<T>(string name)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.GetService<T>(name);
		}

		public bool TryGetService(Type type, out object instance)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.TryGetService(type, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.TryGetService(type, name, out instance);
		}

		public bool TryGetService<T>(out T instance)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.TryGetService(out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.TryGetService(name, out instance);
		}

		public IEnumerable<object> GetServices(Type type)
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.GetServices(type);
		}

		public IEnumerable<T> GetServices<T>()
		{
			AssertNotDisposed();
			using (var context = CreateContext())
				return context.GetServices<T>();
		}

		public IServiceProvider ToServiceProvider()
		{
			AssertNotDisposed();
			return _serviceProvider ?? (_serviceProvider = new ServiceProvider(TryGetService));
		}

		public virtual void Dispose()
		{
			Disposed = true;

			foreach (var disposable in Cache.Values.OfType<IDisposable>())
				disposable.Dispose();
		}

		protected abstract Context CreateContext();

		protected void AssertNotDisposed()
		{
			if (!Disposed) return;
			throw new ObjectDisposedException($"This {GetType().Name} has been disposed.");
		}
	}
}