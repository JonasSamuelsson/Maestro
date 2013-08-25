using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Maestro
{
	public class Container : IContainer, IDependencyContainer
	{
		private static IContainer _default;
		private readonly ICustomDictionary<IPlugin> _plugins;
		private readonly ICustomDictionary<IPipelineEngine> _fallbackPipelines;
		private long _requestId;
		private int _configId;

		public Container()
		{
			_plugins = new PluginDictionary();
			_fallbackPipelines = new FallbackPipelineDictionary();
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
			_fallbackPipelines.Clear();
			Interlocked.Increment(ref _configId);
		}

		public object Get(Type type, string name = null)
		{
			try
			{
				name = name ?? DefaultName;
				var requestId = Interlocked.Increment(ref _requestId);
				using (var context = new Context(_configId, requestId, name, this))
				using (((TypeStack)context.TypeStack).Push(type))
				{
					IPipelineEngine pipelineEngine;
					if (TryGetPipeline(_plugins, type, context, out pipelineEngine))
						return pipelineEngine.Get(context);

					IPipelineEngine fallbackPipelineEngine;
					if (TryGetFallbackPipeline(_fallbackPipelines, type, out fallbackPipelineEngine))
						return fallbackPipelineEngine.Get(context);
				}

				throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName));
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get {0}-{1}.", name, type.FullName), exception);
			}
		}

		private static bool TryGetFallbackPipeline(ICustomDictionary<IPipelineEngine> fallbackPipelines, Type type, out IPipelineEngine pipelineEngine)
		{
			pipelineEngine = null;

			if (!type.IsConcreteClosedClass())
				return false;

			pipelineEngine = fallbackPipelines.GetOrAdd(type);
			return true;
		}

		public T Get<T>(string name = null)
		{
			return (T)Get(typeof(T), name);
		}

		public IEnumerable<object> GetAll(Type type)
		{
			try
			{
				IPlugin plugin;
				if (!_plugins.TryGet(type, out plugin))
					return GetEmptyEnumerableOf(type);

				var requestId = Interlocked.Increment(ref _requestId);
				using (var context = new Context(_configId, requestId, DefaultName, this))
				{
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
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get all {0}.", type.FullName), exception);
			}
		}

		public IEnumerable<T> GetAll<T>()
		{
			return GetAll(typeof(T)).Cast<T>().ToList();
		}

		bool IDependencyContainer.CanGet(Type type, IContext context)
		{
			IPipelineEngine pipelineEngine;
			if (TryGetPipeline(_plugins, type, context, out pipelineEngine))
				using (((TypeStack)context.TypeStack).Push(type))
					return pipelineEngine.CanGet(context);

			Type enumerableType;
			if (TryGetEnumerableType(_plugins, type, out enumerableType))
				return true;

			if (TryGetFallbackPipeline(_fallbackPipelines, type, out pipelineEngine))
				using (((TypeStack)context.TypeStack).Push(type))
					return pipelineEngine.CanGet(context);

			return false;
		}

		object IDependencyContainer.Get(Type type, IContext context)
		{
			try
			{
				var instance = GetDependency(type, context);

				if (!type.IsArray || instance.GetType().IsArray)
					return instance;

				var elementType = type.GetElementType();
				var objects = ((IEnumerable<object>)instance).ToList();
				var array = Array.CreateInstance(elementType, objects.Count);
				for (var i = 0; i < objects.Count; i++)
					array.SetValue(objects[i], i);

				return array;
			}
			catch (ActivationException)
			{
				throw;
			}
			catch (Exception exception)
			{
				throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName), exception);
			}
		}

		private object GetDependency(Type type, IContext context)
		{
			IPipelineEngine pipelineEngine;
			if (TryGetPipeline(_plugins, type, context, out pipelineEngine))
				using (((TypeStack)context.TypeStack).Push(type))
					return pipelineEngine.Get(context);

			Type enumerableType;
			if (TryGetEnumerableType(_plugins, type, out enumerableType))
				return ((IDependencyContainer)this).GetAll(enumerableType, context);

			if (TryGetFallbackPipeline(_fallbackPipelines, type, out pipelineEngine))
				using (((TypeStack)context.TypeStack).Push(type))
					return pipelineEngine.Get(context);

			throw new ActivationException(string.Format("Can't get dependency {0}-{1}.", context.Name, type.FullName));
		}

		private static bool TryGetPipeline(ICustomDictionary<IPlugin> plugins, Type type, IContext context,
			out IPipelineEngine pipelineEngine)
		{
			pipelineEngine = null;

			IPlugin plugin;
			if (!plugins.TryGet(type, out plugin))
				return type.IsGenericType && TryGetGenericPipeline(plugins, type, context, out pipelineEngine);

			if (plugin.TryGet(context.Name, out pipelineEngine))
				return true;

			if (context.Name != DefaultName && plugin.TryGet(DefaultName, out pipelineEngine))
				return true;

			return false;
		}

		private static bool TryGetGenericPipeline(ICustomDictionary<IPlugin> plugins, Type type, IContext context, out IPipelineEngine pipelineEngine)
		{
			pipelineEngine = null;

			lock (plugins)
			{
				IPlugin plugin;
				if (plugins.TryGet(type, out plugin))
					return TryGetPipeline(plugins, type, context, out pipelineEngine);

				var typeDefinition = type.GetGenericTypeDefinition();
				if (!plugins.TryGet(typeDefinition, out plugin))
					return false;

				var genericArguments = type.GetGenericArguments();
				var names = plugin.GetNames().ToList();
				var genericPlugin = plugins.GetOrAdd(type);
				foreach (var name in names)
					genericPlugin.Add(name, plugin.Get(name).MakeGenericPipelineEngine(genericArguments));
			}

			return TryGetPipeline(plugins, type, context, out pipelineEngine);
		}

		private static bool TryGetEnumerableType(ICustomDictionary<IPlugin> plugins, Type type, out Type enumerableType)
		{
			enumerableType = null;

			if (type.IsArray)
			{
				var elementType = type.GetElementType();
				if (!elementType.IsValueType || plugins.Contains(elementType))
				{
					enumerableType = elementType;
					return true;
				}
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				var genericArgument = type.GetGenericArguments().Single();
				if (!genericArgument.IsValueType || plugins.Contains(genericArgument))
					enumerableType = genericArgument;
				return true;
			}

			return false;
		}

		IEnumerable<object> IDependencyContainer.GetAll(Type type, IContext context)
		{
			try
			{
				IPlugin plugin;
				if (!_plugins.TryGet(type, out plugin))
					return GetEmptyEnumerableOf(type);

				using (((TypeStack)context.TypeStack).Push(type))
					return plugin.GetNames().Select(x => plugin.Get(x).Get(context)).ToList();
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
			return (IEnumerable<object>)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
		}
	}
}