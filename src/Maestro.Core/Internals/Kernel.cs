using System;
using System.Collections.Generic;
using System.Linq;

/*
IPlugin
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
	ctor(IPlugin)
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
		private readonly IPluginLookup _plugins;
		private readonly IPipelineLookup _pipelines = new PipelineLookup();

		public Kernel(IPluginLookup plugins)
		{
			_plugins = plugins;
		}

		public bool CanGet(Type type, string name)
		{
			return false;
		}

		public bool TryGet(Type type, string name, Context context, out object instance)
		{
			instance = null;
			var key = $"{type.FullName}-{name}";
			IPipeline pipeline;
			if (!_pipelines.TryGet(key, out pipeline))
			{
				lock (_pipelines)
				{
					if (!_pipelines.TryGet(key, out pipeline))
					{
						IPlugin plugin;
						if (!_plugins.TryGet(type, name, out plugin))
						{
							return false;
							//var resolver = _instanceFactoryResolvers.FirstOrDefault(x => x.CanHandle(type));
							//if (resolver == null) return false;
							//var instanceFactory = resolver.GetInstanceFactory(type);
							//plugin = Create.Instance<IPlugin>(instanceFactory);
						}

						pipeline = new Pipeline(plugin);
						_pipelines.Add(key, pipeline);
					}
				}
			}

			instance = pipeline.Execute(context);
			return true;
		}

		public IEnumerable<object> GetAll(Type type, Context context)
		{
			var key = type.FullName;
			IEnumerable<IPipeline> builders;
			if (!_pipelines.TryGet(key, out builders))
			{
				lock (_pipelines)
				{
					if (!_pipelines.TryGet(key, out builders))
					{
						var plugins = _plugins.GetAll(type);
						builders = plugins.Select(x => new Pipeline(x)).ToList();
						_pipelines.Add(key, builders);
					}
				}
			}

			return builders.Select(x => x.Execute(context)).ToList();
		}
	}
}