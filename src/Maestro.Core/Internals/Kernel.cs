using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders;
using Maestro.TypeFactoryResolvers;

namespace Maestro.Internals
{
	internal class Kernel
	{
		private readonly PluginLookup _pluginLookup;
		private readonly PipelineLookup _pipelineLookup;
		private readonly IEnumerable<IFactoryProviderResolver> _factoryProviderResolvers;

		public Kernel() : this(new PluginLookup())
		{ }

		public Kernel(PluginLookup pluginLookup)
		{
			_pluginLookup = pluginLookup;
			_pipelineLookup = new PipelineLookup();
			_factoryProviderResolvers = new IFactoryProviderResolver[]
											{
												new FuncFactoryProviderResolver(),
												new LazyFactoryProviderResolver(),
												new ConcreteClosedClassFactoryProviderResolver()
											};

			_pluginLookup.PluginAdded += () => _pipelineLookup.Clear();
		}

		public void Add(Plugin plugin)
		{
			lock (_pipelineLookup)
				_pluginLookup.Add(plugin);
		}

		public Kernel GetChildKernel()
		{
			return new Kernel(new PluginLookup(_pluginLookup));
		}

		public object Get(Type type, string name)
		{
			name = name ?? PluginLookup.DefaultName;
			object instance;
			if (TryGet(type, name, out instance))
				return instance;

			throw new NotImplementedException("foobar");
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
				if (!_pipelineLookup.TryGet(key, out pipelines))
				{
					lock (_pipelineLookup)
					{
						if (!_pipelineLookup.TryGet(key, out pipelines))
						{
							var plugins = _pluginLookup.GetAll(type);
							pipelines = plugins.Select(x => new Pipeline(x)).ToList();
							_pipelineLookup.Add(key, pipelines);
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
			return CanGet(type, context) || IsEnumerable(type);
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

			if (IsEnumerable(type))
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
			var key = GetKey(type, context);
			if (!_pipelineLookup.TryGet(key, out pipeline))
			{
				lock (_pipelineLookup)
				{
					if (!_pipelineLookup.TryGet(key, out pipeline))
					{
						Plugin plugin;
						if (_pluginLookup.TryGet(type, context.Name, out plugin))
						{
							pipeline = new Pipeline(plugin);
							_pipelineLookup.Add(key, pipeline);
							return true;
						}

						if (type.IsGenericType)
						{
							var genericTypeDefinition = type.GetGenericTypeDefinition();
							if (genericTypeDefinition == typeof(IEnumerable<>))
							{
								var elementType = type.GetGenericArguments().Single();
								var isPrimitive = elementType.IsValueType || elementType == typeof (string) || elementType == typeof (object);
								var pipelines = GetPipelines(elementType).ToList();
								if (!isPrimitive || pipelines.Count != 0)
								{
									pipeline = new EnumerablePipeline(elementType, pipelines);
									_pipelineLookup.Add(key, pipeline);
									return true;
								}
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
							_pipelineLookup.Add(key, pipeline);
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
			var key = type.FullName;
			IEnumerable<IPipeline> pipelines;

			if (_pipelineLookup.TryGet(key, out pipelines))
				return pipelines;

			lock (_pipelineLookup)
			{
				if (_pipelineLookup.TryGet(key, out pipelines))
					return pipelines;

				var plugins = _pluginLookup.GetAll(type);
				pipelines = plugins.Select(x => new Pipeline(x)).ToList();
				_pipelineLookup.Add(key, pipelines);
				return pipelines;
			}
		}

		private static string GetKey(Type type, Context context)
		{
			var key = $"{type.FullName}-{context.Name}";
			return key;
		}

		private static bool IsEnumerable(Type type)
		{
			if (!type.IsGenericType) return false;
			var genericTypeDefinition = type.GetGenericTypeDefinition();
			if (genericTypeDefinition != typeof(IEnumerable<>)) return false;
			var genericArgument = type.GetGenericArguments().Single();
			return genericArgument != typeof(string) && !genericArgument.IsValueType;
		}
	}
}