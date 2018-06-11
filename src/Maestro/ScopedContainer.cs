using Maestro.Diagnostics;
using Maestro.Internals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	public class ScopedContainer : IScopedContainer
	{
		internal ScopedContainer()
		{
			Kernel = new Kernel();
			RootScope = new ConcurrentDictionary<object, Lazy<object>>();
			CurrentScope = RootScope;
		}

		internal ScopedContainer(Kernel kernel, ConcurrentDictionary<object, Lazy<object>> rootScope)
		{
			Kernel = kernel;
			RootScope = rootScope;
			CurrentScope = new ConcurrentDictionary<object, Lazy<object>>();
		}

		public IDiagnostics Diagnostics => new Diagnostics.Diagnostics(Kernel);
		internal Kernel Kernel { get; }
		internal ConcurrentDictionary<object, Lazy<object>> CurrentScope { get; }
		internal ConcurrentDictionary<object, Lazy<object>> RootScope { get; }

		public object GetService(Type type)
		{
			using (var context = new Context(this, Kernel))
				return context.GetService(type, ServiceNames.Default);
		}

		public object GetService(Type type, string name)
		{
			using (var context = new Context(this, Kernel))
				return context.GetService(type, name);
		}

		public T GetService<T>()
		{
			using (var context = new Context(this, Kernel))
				return context.GetService<T>(ServiceNames.Default);
		}

		public T GetService<T>(string name)
		{
			using (var context = new Context(this, Kernel))
				return context.GetService<T>(name);
		}

		public bool TryGetService(Type type, out object instance)
		{
			using (var context = new Context(this, Kernel))
				return context.TryGetService(type, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			using (var context = new Context(this, Kernel))
				return context.TryGetService(type, name, out instance);
		}

		public bool TryGetService<T>(out T instance)
		{
			using (var context = new Context(this, Kernel))
				return context.TryGetService(out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			using (var context = new Context(this, Kernel))
				return context.TryGetService(name, out instance);
		}

		public IEnumerable<object> GetServices(Type type)
		{
			using (var context = new Context(this, Kernel))
				return context.GetServices(type);
		}

		public IEnumerable<T> GetServices<T>()
		{
			using (var context = new Context(this, Kernel))
				return context.GetServices<T>();
		}

		public void Dispose()
		{
			// todo perf no linq
			CurrentScope.Values
				.Where(x => x.IsValueCreated)
				.Select(x => x.Value)
				.OfType<IDisposable>()
				.ForEach(x => x.Dispose());

			Kernel.Dispose();
		}
	}
}