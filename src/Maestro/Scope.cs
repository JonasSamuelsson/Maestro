using Maestro.Internals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Maestro
{
	public class Scope : IScope
	{
		private IServiceProvider _serviceProvider;

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
			using (var context = CreateContext())
				return context.GetService(type, ServiceNames.Default);
		}

		public object GetService(Type type, string name)
		{
			using (var context = CreateContext())
				return context.GetService(type, name);
		}

		public T GetService<T>()
		{
			using (var context = CreateContext())
				return context.GetService<T>(ServiceNames.Default);
		}

		public T GetService<T>(string name)
		{
			using (var context = CreateContext())
				return context.GetService<T>(name);
		}

		public bool TryGetService(Type type, out object instance)
		{
			using (var context = CreateContext())
				return context.TryGetService(type, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			using (var context = CreateContext())
				return context.TryGetService(type, name, out instance);
		}

		public bool TryGetService<T>(out T instance)
		{
			using (var context = CreateContext())
				return context.TryGetService(out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			using (var context = CreateContext())
				return context.TryGetService(name, out instance);
		}

		public IEnumerable<object> GetServices(Type type)
		{
			using (var context = CreateContext())
				return context.GetServices(type);
		}

		public IEnumerable<T> GetServices<T>()
		{
			using (var context = CreateContext())
				return context.GetServices<T>();
		}

		public Scope CreateScope()
		{
			return new Scope(Kernel, RootScope);
		}

		IScope IScope.CreateScope() => CreateScope();

		public IServiceProvider ToServiceProvider()
		{
			return _serviceProvider ?? (_serviceProvider = new ServiceProvider(TryGetService));
		}

		public void Dispose()
		{
			foreach (var kvp in Cache.ToArray())
				if (kvp.Value is IDisposable disposable)
					disposable.Dispose();

			if (this == RootScope)
				Kernel.Dispose();
		}

		private Context CreateContext() => new Context(Kernel, this, RootScope);
	}
}