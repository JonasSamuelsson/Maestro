using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.FactoryProviders;
using Maestro.TypeFactoryResolvers;
using Maestro.Utils;

namespace Maestro.Internals
{
	internal class Kernel : IDisposable
	{
		private readonly PluginLookup _pluginLookup;
		private readonly PipelineCache _pipelineCache;
		private readonly IEnumerable<IFactoryProviderResolver> _factoryProviderResolvers;
		private readonly Kernel _parent;

		public event EventHandler ConfigurationChanged;

		public Kernel() : this(new PluginLookup())
		{ }

		public Kernel(PluginLookup pluginLookup)
		{
			_pluginLookup = pluginLookup;
			_pipelineCache = new PipelineCache();
			_factoryProviderResolvers = new IFactoryProviderResolver[]
											{
												new FuncFactoryProviderResolver(),
												new LazyFactoryProviderResolver(),
												new ConcreteClosedClassFactoryProviderResolver()
											};

			_pluginLookup.PluginAdded += () =>
			{
				_pipelineCache.Clear();
				ConfigurationChanged?.Invoke(this, EventArgs.Empty);
			};
		}

		public Kernel(Kernel kernel) : this()
		{
			_parent = kernel;
			_parent.ConfigurationChanged += ParentConfigurationChanged;
		}

		private void ParentConfigurationChanged(object sender, EventArgs e)
		{
			lock (_pipelineCache)
				_pipelineCache.Clear();
		}

		public bool Add(ServiceDescriptor serviceDescriptor, bool throwIfDuplicate)
		{
			lock (_pipelineCache)
				return _pluginLookup.Add(serviceDescriptor, throwIfDuplicate);
		}

		public bool CanGetService(Type type, Context context)
		{
			IPipeline pipeline;
			return TryGetPipeline(type, context, out pipeline) || Reflector.IsGenericEnumerable(type);
		}

		public bool TryGetService(Type type, Context context, out object instance)
		{
			IPipeline pipeline;
			instance = TryGetPipeline(type, context, out pipeline) ? pipeline.Execute(context) : null;

			if (instance == null && Reflector.IsGenericEnumerable(type))
			{
				// todo performance
				var elementType = type.GetGenericArguments().Single();
				instance = Array.CreateInstance(elementType, 0);
			}

			return instance != null;
		}

		public IEnumerable<object> GetServices(Type type, Context context)
		{
			var enumerableType = typeof(IEnumerable<>).MakeGenericType(type);

			object instance;
			if (TryGetService(enumerableType, context, out instance))
			{
				return (IEnumerable<object>)instance;
			}

			throw new InvalidOperationException();
		}

		private bool TryGetPipeline(Type type, Context context, out IPipeline pipeline)
		{
			var pipelineKey = GetPipelineKey(type, context);
			if (!_pipelineCache.TryGet(pipelineKey, out pipeline))
			{
				lock (_pipelineCache)
				{
					if (!_pipelineCache.TryGet(pipelineKey, out pipeline))
					{
						var name = context.Name;

						tryGetPlugin:
						for (var kernel = this; kernel != null; kernel = kernel._parent)
						{
							ServiceDescriptor serviceDescriptor;
							if (kernel._pluginLookup.TryGet(type, name, out serviceDescriptor))
							{
								pipeline = new Pipeline(serviceDescriptor);
								_pipelineCache.Add(pipelineKey, pipeline);
								return true;
							}

							if (Reflector.IsGenericEnumerable(type))
							{
								var elementType = type.GetGenericArguments().Single();
								var plugins = _pluginLookup.GetAll(elementType);
								var pipelines = plugins.Select(x => new Pipeline(x)).ToList();

								if (pipelines.Any())
								{
									pipeline = new EnumerablePipeline(elementType, pipelines);
									_pipelineCache.Add(pipelineKey, pipeline);
									return true;
								}
							}

							if (name != PluginLookup.DefaultName)
							{
								name = PluginLookup.DefaultName;
								goto tryGetPlugin;
							}
						}

						foreach (var factoryProviderResolver in _factoryProviderResolvers)
						{
							IFactoryProvider factoryProvider;
							if (!factoryProviderResolver.TryGet(type, context, out factoryProvider)) continue;
							pipeline = new Pipeline
							{
								ServiceDescriptor = new ServiceDescriptor
								{
									Type = type,
									Name = context.Name,
									FactoryProvider = factoryProvider
								}
							};
							_pipelineCache.Add(pipelineKey, pipeline);
							return true;
						}

						return false;
					}
				}
			}

			return true;
		}

		private static string GetPipelineKey(Type type, Context context)
		{
			return $"{type.FullName}:{context.Name}";
		}

		public void Dispose()
		{
			if (_parent == null) return;
			_parent.ConfigurationChanged -= ParentConfigurationChanged;
		}
	}
}