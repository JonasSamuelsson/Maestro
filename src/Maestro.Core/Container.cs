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
			action(new ContainerExpression(_kernel, _defaultSettings));
		}

		public IContainer GetChildContainer()
		{
			return GetChildContainer(delegate { });
		}

		public IContainer GetChildContainer(Action<IContainerExpression> action)
		{
			var childContainer = new Container(_kernel.GetChildKernel());
			childContainer.Configure(action);
			return childContainer;
		}

		public T Get<T>(string name = null)
		{
			return (T)Get(typeof(T), name);
		}

		public object Get(Type type, string name = null)
		{
			return _kernel.Get(type, name);
		}

		public bool TryGet<T>(out T instance)
		{
			return TryGet(PluginLookup.DefaultName, out instance);
		}

		public bool TryGet<T>(string name, out T instance)
		{
			object temp;
			var result = TryGet(typeof(T), name, out temp);
			instance = (T)temp;
			return result;
		}

		public bool TryGet(Type type, out object instance)
		{
			return TryGet(type, PluginLookup.DefaultName, out instance);
		}

		public bool TryGet(Type type, string name, out object instance)
		{
			return _kernel.TryGet(type, name, out instance);
		}

		public IEnumerable<T> GetAll<T>()
		{
			return GetAll(typeof(T)).Cast<T>().ToList();
		}

		public IEnumerable<object> GetAll(Type type)
		{
			return _kernel.GetAll(type);
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
		}
	}
}