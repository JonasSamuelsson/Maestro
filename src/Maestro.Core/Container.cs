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

		public T GetService<T>(string name = null)
		{
			return (T)GetService(typeof(T), name);
		}

		public object GetService(Type type, string name = null)
		{
			object instance;
			if (TryGetService(type, name, out instance))
				return instance;

			var template = PluginLookup.EqualsDefaultName(name)
									? "Can't get default instance of type '{0}'."
									: "Can't get '{1}' instance of type '{0}'.";
			var message = string.Format(template, type.FullName, name);
			throw new ActivationException(message);
		}

		public bool TryGetService<T>(out T instance)
		{
			return TryGetService(PluginLookup.DefaultName, out instance);
		}

		public bool TryGetService<T>(string name, out T instance)
		{
			object temp;
			var result = TryGetService(typeof(T), name, out temp);
			instance = (T)temp;
			return result;
		}

		public bool TryGetService(Type type, out object instance)
		{
			return TryGetService(type, PluginLookup.DefaultName, out instance);
		}

		public bool TryGetService(Type type, string name, out object instance)
		{
			try
			{
				return _kernel.TryGet(type, name, out instance);
			}
			catch (Exception exception)
			{
				var template = PluginLookup.EqualsDefaultName(name)
										? "Can't get default instance of type '{0}'."
										: "Can't get '{1}' instance of type '{0}'.";
				var message = string.Format(template, type.FullName, name);
				throw new ActivationException(message, exception);
			}
		}

		public IEnumerable<T> GetServices<T>()
		{
			return GetServices(typeof(T)).Cast<T>().ToList();
		}

		public IEnumerable<object> GetServices(Type type)
		{
			try
			{
				return _kernel.GetAll(type);
			}
			catch (Exception exception)
			{
				var message = $"Can't get all instances of type '{type.FullName}'.";
				throw new ActivationException(message, exception);
			}
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