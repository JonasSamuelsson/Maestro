using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		public bool Add(Plugin plugin, bool throwIfDuplicate)
		{
			lock (_pipelineCache)
				return _pluginLookup.Add(plugin, throwIfDuplicate);
		}

		public object Get(Type type, string name)
		{
			name = name ?? PluginLookup.DefaultName;
			object instance;
			if (TryGet(type, name, out instance))
				return instance;

			throw new NotImplementedException("error handling");
		}

		public bool TryGet(Type type, string name, out object instance)
		{
			name = name ?? PluginLookup.DefaultName;
			using (var context = new Context(name, this))
				return TryGet(type, context, out instance);
		}

		public IEnumerable<object> GetAll(Type type)
		{
			var context = new Context(PluginLookup.DefaultName, this);
			return GetAll(type, context);
		}

		public bool CanGet(Type type, Context context)
		{
			context.PushStackFrame(type);

			try
			{
				IPipeline pipeline;
				return TryGetPipeline(type, context, out pipeline);
			}
			finally
			{
				context.PopStackFrame();
			}
		}

		public bool TryGet(Type type, Context context, out object instance)
		{
			context.PushStackFrame(type);

			try
			{
				IPipeline pipeline;
				instance = TryGetPipeline(type, context, out pipeline) ? pipeline.Execute(context) : null;
				return instance != null;
			}
			finally
			{
				context.PopStackFrame();
			}
		}

		public IEnumerable<object> GetAll(Type type, Context context)
		{
			context.PushStackFrame(type);

			try
			{
				var key = type.FullName;
				IEnumerable<IPipeline> pipelines;
				if (!_pipelineCache.TryGet(key, out pipelines))
				{
					lock (_pipelineCache)
					{
						if (!_pipelineCache.TryGet(key, out pipelines))
						{
							var names = new HashSet<string>();
							var list = new List<IPipeline>();
							for (var kernel = this; kernel != null; kernel = kernel._parent)
							{
								var plugins = kernel._pluginLookup.GetAll(type).ToList();
								for (var i = 0; i < plugins.Count; i++)
								{
									var plugin = plugins[i];
									if (!names.Add(plugin.Name)) continue;
									list.Add(new Pipeline(plugin));
								}
							}
							pipelines = list;
							_pipelineCache.Add(key, pipelines);
						}
					}
				}

				return pipelines.Select(x => x.Execute(context)).ToList();
			}
			finally
			{
				context.PopStackFrame();
			}
		}

		public bool CanGetDependency(Type type, Context context)
		{
			return CanGet(type, context) || Reflector.IsEnumerable(type);
		}

		public object GetDependency(Type type, Context context)
		{
			object instance;
			if (TryGetDependency(type, context, out instance)) return instance;
			throw new InvalidOperationException();
		}

		public bool TryGetDependency(Type type, Context context, out object instance)
		{
			if (TryGet(type, context, out instance)) return true;

			if (Reflector.IsEnumerable(type))
			{
				var elementType = type.GetGenericArguments().Single();
				var instances = GetAll(elementType, context);
				var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
				var genericCastMethod = castMethod.MakeGenericMethod(elementType);
				instance = genericCastMethod.Invoke(null, new object[] { instances });
				return true;
			}

			return false;
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
							Plugin plugin;
							if (kernel._pluginLookup.TryGet(type, name, out plugin))
							{
								pipeline = new Pipeline(plugin);
								_pipelineCache.Add(pipelineKey, pipeline);
								return true;
							}

							if (type.IsGenericType)
							{
								var genericTypeDefinition = type.GetGenericTypeDefinition();
								if (genericTypeDefinition == typeof(IEnumerable<>))
								{
									var elementType = type.GetGenericArguments().Single();
									var isPrimitive = elementType.IsValueType || elementType == typeof(string) || elementType == typeof(object);
									var pipelines = GetPipelines(elementType).ToList();
									if (!isPrimitive || pipelines.Count != 0)
									{
										pipeline = new EnumerablePipeline(elementType, pipelines);
										_pipelineCache.Add(pipelineKey, pipeline);
										return true;
									}
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
								Plugin = new Plugin
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

		private IEnumerable<IPipeline> GetPipelines(Type type)
		{
			var key = $"{type.FullName}[]";
			IEnumerable<IPipeline> pipelines;

			if (_pipelineCache.TryGet(key, out pipelines))
				return pipelines;

			lock (_pipelineCache)
			{
				if (_pipelineCache.TryGet(key, out pipelines))
					return pipelines;

				var plugins = _pluginLookup.GetAll(type);
				pipelines = plugins.Select(x => new Pipeline(x)).ToList();
				_pipelineCache.Add(key, pipelines);
				return pipelines;
			}
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