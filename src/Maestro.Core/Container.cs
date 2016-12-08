using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Configuration;
using Maestro.Internals;

namespace Maestro
{
	public class Container : IContainer
	{
		private readonly Guid _id;
		private readonly Kernel _kernel;
		private readonly DefaultSettings _defaultSettings;
		private event Action<Guid> DisposedEvent;

		/// <summary>
		/// Instantiates a new empty container.
		/// </summary>
		public Container() : this(new Kernel())
		{ }

		/// <summary>
		/// Instantiates a new container with configuration.
		/// </summary>
		public Container(Action<IContainerExpression> action)
			: this()
		{
			Configure(action);
		}

		private Container(Kernel kernel)
		{
			_id = Guid.NewGuid();
			_kernel = kernel;
			_defaultSettings = new DefaultSettings();
		}

		public void Configure(Action<IContainerExpression> action)
		{
			using (var containerExpression = new ContainerExpression(_kernel, _defaultSettings))
				action(containerExpression);
		}

		public IContainer GetChildContainer()
		{
			return GetChildContainer(delegate { });
		}

		public IContainer GetChildContainer(Action<IContainerExpression> action)
		{
			var childContainer = new Container(new Kernel(_kernel));
			childContainer.Configure(action);
			return childContainer;
		}

		public object GetService(Type type, string name = null)
		{
			using (var context = new Context(name, _kernel))
				return context.GetService(type);
		}

		public T GetService<T>(string name = null)
		{
			using (var context = new Context(name, _kernel))
				return context.GetService<T>();
		}

		public bool TryGetService(Type type, out object instance)
		{
			using (var context = new Context(PluginLookup.DefaultName, _kernel))
				return context.TryGetService(type, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			using (var context = new Context(name, _kernel))
				return context.TryGetService(type, out instance);
		}

		public bool TryGetService<T>(out T instance)
		{
			using (var context = new Context(PluginLookup.DefaultName, _kernel))
				return context.TryGetService(out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			using (var context = new Context(name, _kernel))
				return context.TryGetService(out instance);
		}

		public IEnumerable<object> GetServices(Type type)
		{
			using (var context = new Context(PluginLookup.DefaultName, _kernel))
				return context.GetServices(type);
		}

		public IEnumerable<T> GetServices<T>()
		{
			return GetServices(typeof(T)).Cast<T>().ToList();
		}

		public string GetConfiguration()
		{
			throw new NotImplementedException();
			//var builder = new DiagnosticsBuilder();
			//foreach (var pair1 in _plugins.OrderBy(x => x.Key.FullName))
			//{
			//	using (builder.Category(pair1.Key))
			//		foreach (var pair2 in pair1.Value.OrderBy(x => x.Key))
			//		{
			//			var prefix = pair2.Key == DefaultName ? "{default}" : pair2.Key;
			//			builder.Prefix(prefix + " : ");
			//			pair2.Value.GetConfiguration(builder);
			//		}
			//	builder.Line();
			//}
			//return builder.ToString();
		}

		public event Action<Guid> Disposed
		{
			add { DisposedEvent += value; }
			remove { DisposedEvent -= value; }
		}

		public void Dispose()
		{
			DisposedEvent?.Invoke(_id);
			_kernel.Dispose();
		}
	}
}