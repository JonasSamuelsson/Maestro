using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maestro
{
	public class Container : IContainer, IDependencyContainer
	{
		private static IContainer _default;
		private readonly IPluginDictionary _plugins;
		private long _requestId;

		public Container()
		{
			_plugins = new PluginDictionary();
		}

		public Container(Action<IContainerConfiguration> action)
			: this()
		{
			Configure(action);
		}

		public static IContainer Default
		{
			get { return _default ?? (_default = new Container()); }
		}

		internal static string DefaultName { get { return "default"; } }

		public void Configure(Action<IContainerConfiguration> action)
		{
			action(new ContainerConfiguration(_plugins));
		}

		public object Get(Type type, string name = null)
		{
			try
			{
				TypeHelper.AssertTypeIsSupported(type);

				name = name ?? DefaultName;
				var requestId = Interlocked.Increment(ref _requestId);
				var context = new Context(requestId, name, this);

				IPipeline pipeline;
				if (TryGetPipeline(_plugins, type, context, out pipeline))
					return pipeline.Get(context);
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName), exception);
			}

			throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName));
		}

		public T Get<T>(string name = null)
		{
			return (T)Get(typeof(T), name);
		}

		public IEnumerable<object> GetAll(Type type)
		{
			try
			{
				TypeHelper.AssertTypeIsSupported(type);

				IPlugin plugin;
				if (!_plugins.TryGet(type, out plugin))
					return GetEmptyEnumerableOf(type);

				var requestId = Interlocked.Increment(ref _requestId);
				var context = new Context(requestId, DefaultName, this);

				var names = plugin.GetNames().ToList();
				var list = new List<object>(names.Count());

				foreach (var name in names)
				{
					context.Name = name;
					var instance = plugin.Get(name).Get(context);
					list.Add(instance);
				}

				return list;
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get all {0}.", type.FullName), exception);
			}

			throw new ActivationException(string.Format("Can't get all {0}.", type.FullName));
		}

		public IEnumerable<T> GetAll<T>()
		{
			return GetAll(typeof(T)).Cast<T>().ToList();
		}

		bool IDependencyContainer.CanGet(Type type, IContext context)
		{
			if (type.IsValueType)
				return false;

			IPipeline pipeline;
			if (TryGetPipeline(_plugins, type, context, out pipeline))
				return true;

			Type enumerableType;
			if (TryGetEnumerableType(type, out enumerableType))
				return true;

			return false;
		}

		object IDependencyContainer.Get(Type type, IContext context)
		{
			try
			{
				TypeHelper.AssertTypeIsSupported(type);

				IPipeline pipeline;
				if (TryGetPipeline(_plugins, type, context, out pipeline))
					return pipeline.Get(context);

				Type enumerableType;
				if (TryGetEnumerableType(type, out enumerableType))
					return ((IDependencyContainer)this).GetAll(enumerableType, context);
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName), exception);
			}

			throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName));
		}

		private static bool TryGetPipeline(IPluginDictionary plugins, Type type, IContext context,
			out IPipeline pipeline)
		{
			pipeline = null;

			IPlugin plugin;
			if (!plugins.TryGet(type, out plugin))
				return false;

			if (plugin.TryGet(context.Name, out pipeline))
				return true;

			if (context.Name != DefaultName && plugin.TryGet(DefaultName, out pipeline))
				return true;

			return false;
		}

		private static bool TryGetEnumerableType(Type type, out Type enumerableType)
		{
			enumerableType = null;

			if (type.IsArray)
			{
				var elementType = type.GetElementType();
				if (!elementType.IsValueType)
				{
					enumerableType = elementType;
					return true;
				}
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				enumerableType = type.GetGenericArguments().Single();
				return true;
			}

			return false;
		}

		IEnumerable<object> IDependencyContainer.GetAll(Type type, IContext context)
		{
			try
			{
				IPlugin plugin;
				return !_plugins.TryGet(type, out plugin)
					? GetEmptyEnumerableOf(type)
					: plugin.GetNames().Select(x => plugin.Get(x).Get(context)).ToArray();
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get all dependencies {0}-{1}.", context.Name, type.FullName), exception);
			}
		}

		private static IEnumerable<object> GetEmptyEnumerableOf(Type type)
		{
			return (IEnumerable<object>)Activator.CreateInstance(type.MakeArrayType(), 0);
		}
	}
}