using Maestro.Configuration;
using Maestro.Diagnostics;
using Maestro.Internals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	public class Container : IContainer
	{
		private readonly Kernel _kernel;

		/// <summary>
		/// Instantiates a new empty container.
		/// </summary>
		public Container() : this(new Kernel(), new ConcurrentDictionary<object, Lazy<object>>())
		{
			CurrentScope = RootScope;
		}

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(Action<ContainerExpression> action)
			: this()
		{
			Configure(action);
		}

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(ContainerBuilder builder)
			: this()
		{
			builder.Configure(this);
		}

		private Container(Kernel kernel, ConcurrentDictionary<object, Lazy<object>> rootScope)
		{
			_kernel = kernel;
			RootScope = rootScope;
			CurrentScope = new ConcurrentDictionary<object, Lazy<object>>();
		}

		public IDiagnostics Diagnostics => new Diagnostics.Diagnostics(_kernel);
		internal ConcurrentDictionary<object, Lazy<object>> CurrentScope { get; }
		internal ConcurrentDictionary<object, Lazy<object>> RootScope { get; }

		public void Configure(Action<ContainerExpression> action)
		{
			var containerExpression = new ContainerExpression(_kernel);
			try
			{
				action(containerExpression);
			}
			finally
			{
				containerExpression.Dispose();
			}
		}

		public void Configure(ContainerBuilder builder)
		{
			builder.Configure(this);
		}

		public IContainer GetChildContainer()
		{
			return GetChildContainer(delegate { });
		}

		public IContainer GetChildContainer(Action<ContainerExpression> action)
		{
			var childContainer = new Container(new Kernel(_kernel), RootScope);
			childContainer.Configure(action);
			return childContainer;
		}

		public IContainer GetChildContainer(ContainerBuilder builder)
		{
			var childContainer = GetChildContainer();
			builder.Configure(childContainer);
			return childContainer;
		}

		public object GetService(Type type)
		{
			using (var context = new Context(this, _kernel))
				return context.GetService(type, ServiceNames.Default);
		}

		public object GetService(Type type, string name)
		{
			using (var context = new Context(this, _kernel))
				return context.GetService(type, name);
		}

		public T GetService<T>()
		{
			using (var context = new Context(this, _kernel))
				return context.GetService<T>(ServiceNames.Default);
		}

		public T GetService<T>(string name)
		{
			using (var context = new Context(this, _kernel))
				return context.GetService<T>(name);
		}

		public bool TryGetService(Type type, out object instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(type, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(type, name, out instance);
		}

		public bool TryGetService<T>(out T instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			using (var context = new Context(this, _kernel))
				return context.TryGetService(name, out instance);
		}

		public IEnumerable<object> GetServices(Type type)
		{
			using (var context = new Context(this, _kernel))
				return context.GetServices(type);
		}

		public IEnumerable<T> GetServices<T>()
		{
			using (var context = new Context(this, _kernel))
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

			_kernel.Dispose();
		}
	}
}