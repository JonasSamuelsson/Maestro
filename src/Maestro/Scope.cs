using Maestro.Internals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	public class Scope : IScope
	{
		private IServiceProvider _serviceProvider;
		protected bool Disposed { get; private set; }

		internal Scope()
		{
			Cache = new ConcurrentDictionary<object, object>();
			Kernel = new Kernel();
			RootScope = this;
		}

		internal Scope(Kernel kernel, Scope rootScope)
		{
			Cache = new ConcurrentDictionary<object, object>();
			Kernel = kernel;
			RootScope = rootScope;
		}

		internal ConcurrentDictionary<object, object> Cache { get; }
		internal Kernel Kernel { get; }
		internal Scope RootScope { get; }

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

		public Scope CreateScope()
		{
			AssertNotDisposed();
			return new Scope(Kernel, RootScope);
		}

		IScope IScope.CreateScope()
		{
			return CreateScope();
		}

		public IServiceProvider ToServiceProvider()
		{
			AssertNotDisposed();
			return _serviceProvider ?? (_serviceProvider = new ServiceProvider(TryGetService));
		}

		public virtual void Dispose()
		{
			if (Disposed)
				return;

			Disposed = true;

			foreach (var disposable in Cache.Values.OfType<IDisposable>())
				disposable.Dispose();

			if (this == RootScope)
				Kernel.Dispose();
		}

		private Context CreateContext()
		{
			return new Context(Kernel, this, RootScope);
		}

		protected void AssertNotDisposed()
		{
			if (!Disposed) return;
			throw new ObjectDisposedException($"{GetType().Name} is disposed.");
		}
	}
}