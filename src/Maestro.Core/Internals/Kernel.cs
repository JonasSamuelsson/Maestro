using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.TypeFactoryResolvers;

/*
Plugin
	IProvider : type/lambda/instance
	IInterceptor
	ILifetime
IProvider
	Func<Context,object> GetProviderMethod()
TypeProvider : IProvider
	ctor(type)
	CtorParameterTypes
	CtorParameterValues
IBuilder / IPipeline
	ctor(Plugin)
	object Execute(IContext)
IProviderResolver
	Concrete closed class
	Func<T>
	Lacy<T>
Kernel
	ctor(plugins)
	GetBuilder(type,name)
	GetBuilders(type)
IContext
	CanGet(type)
	TryGet(type)
	Get(type)
	GetAll(type)
Context : IContext
	ctor(Kernel)
	Kernel
*/

namespace Maestro.Internals
{
	internal class Kernel
	{
		private readonly IPipelineLookup _pipelines;
		private readonly IEnumerable<ITypeFactoryResolver> _typeFactoryResolvers;

		public Kernel() : this(new PluginLookup())
		{ }

		public Kernel(PluginLookup plugins)
		{
			Plugins = plugins;
			_pipelines = new PipelineLookup();
			_typeFactoryResolvers = new ITypeFactoryResolver[]
											{
												new FuncFactoryResolver(),
												new LazyFactoryResolver(),
												new ConcreteClosedClassTypeFactoryResolver()
											};
		}

		public PluginLookup Plugins { get; }

		public object Get(Type type, string name)
		{
			name = name ?? PluginLookup.DefaultName;
			object instance;
			if (TryGet(type, name, out instance))
				return instance;

			var message = name == PluginLookup.DefaultName
								  ? $"Can't get default instance of type {type.FullName}."
								  : $"Can't get '{name}' instance of type {type.FullName}.";
			throw new ActivationException(message);
		}

		public bool TryGet(Type type, string name, out object instance)
		{
			try
			{
				instance = null;
				name = name ?? PluginLookup.DefaultName;
				using (var context = new Context(name, this))
					return TryGet(type, context, out instance);
			}
			catch (Exception exception)
			{
				var message = name == PluginLookup.DefaultName
									  ? $"Can't get default instance of type {type.FullName}."
									  : $"Can't get '{name}' instance of type {type.FullName}.";
				throw new ActivationException(message, exception);
			}
		}

		public IEnumerable<object> GetAll(Type type)
		{
			try
			{
				var context = new Context(PluginLookup.DefaultName, this);
				return GetAll(type, context);
			}
			catch (Exception exception)
			{
				var message = $"Can't get instances of type {type.FullName}.";
				throw new ActivationException(message, exception);
			}
		}

		public bool CanGet(Type type, Context context)
		{
			context.PushStackFrame(type);

			try
			{
				Pipeline pipeline;
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
				Pipeline pipeline;
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
				IEnumerable<Pipeline> builders;
				if (!_pipelines.TryGet(key, out builders))
				{
					lock (_pipelines)
					{
						if (!_pipelines.TryGet(key, out builders))
						{
							var plugins = Plugins.GetAll(type);
							builders = plugins.Select(x => new Pipeline(x)).ToList();
							_pipelines.Add(key, builders);
						}
					}
				}

				return builders.Select(x => x.Execute(context)).ToList();
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
				instance = GetAll(type.GetGenericArguments().Single(), context);
				return true;
			}

			return false;
		}

		private bool TryGetPipeline(Type type, Context context, out Pipeline pipeline)
		{
			var key = GetKey(type, context);
			if (!_pipelines.TryGet(key, out pipeline))
			{
				lock (_pipelines)
				{
					if (!_pipelines.TryGet(key, out pipeline))
					{
						Plugin plugin;
						if (Plugins.TryGet(type, context.Name, out plugin))
						{
							pipeline = new Pipeline(plugin);
							_pipelines.Add(key, pipeline);
							return true;
						}

						foreach (var typeFactoryResolver in _typeFactoryResolvers)
						{
							if (!typeFactoryResolver.TryGet(type, context, out pipeline)) continue;
							_pipelines.Add(key, pipeline);
							return true;
						}

						return false;
					}
				}
			}

			return true;
		}

		private static string GetKey(Type type, Context context)
		{
			var key = $"{type.FullName}-{context.Name}";
			return key;
		}

		private static bool IsEnumerable(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
		}
	}
}