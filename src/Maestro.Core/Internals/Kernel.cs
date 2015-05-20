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
	static class Create
	{
		public static T Instance<T>(params object[] parameters)
		{
			throw new NotImplementedException();
		}
	}

	internal class Cntnr
	{
		private Kernel _kernel = new Kernel();

		public object Get(Type type, string name)
		{
			object instance;
			var ctx = Create.Instance<ICtx>();
			if (_kernel.TryGet(type, name, ctx, out instance)) return instance;
			throw new InvalidOperationException();
		}

		public IEnumerable<object> GetAll(Type type)
		{
			var ctx = Create.Instance<ICtx>();
			return _kernel.GetAll(type, ctx);
		}
	}

	internal interface ICtx
	{
		IDisposable PushFrame(Type type);
	}

	internal class Kernel
	{
		private readonly IPluginLookup _plugins = Create.Instance<IPluginLookup>();
		private readonly IBuilderLookup _builders = Create.Instance<IBuilderLookup>();
		private readonly IEnumerable<IInstanceFactoryResolver> _instanceFactoryResolvers = Create.Instance<IEnumerable<IInstanceFactoryResolver>>();

		public bool TryGet(Type type, string name, ICtx ctx, out object instance)
		{
			using (ctx.PushFrame(type))
			{
				instance = null;
				var key = $"{type.FullName}-{name}";
				IBuilder builder;
				if (!_builders.TryGet(key, out builder))
				{
					lock (_builders)
					{
						if (!_builders.TryGet(key, out builder))
						{
							IPlugin plugin;
							if (!_plugins.TryGet(type, name, out plugin))
							{
								var resolver = _instanceFactoryResolvers.FirstOrDefault(x => x.CanHandle(type));
								if (resolver == null) return false;
								var instanceFactory = resolver.GetInstanceFactory(type);
								plugin = Create.Instance<IPlugin>(instanceFactory);
							}

							builder = Create.Instance<IBuilder>(plugin);
							_builders.Add(key, builder);
						}
					}
				}

				instance = builder.Execute();
				return true;
			}
		}

		public IEnumerable<object> GetAll(Type type, ICtx ctx)
		{
			using (ctx.PushFrame(type))
			{
				var key = type.FullName;
				IEnumerable<IBuilder> builders;
				if (!_builders.TryGet(key, out builders))
				{
					lock (_builders)
					{
						if (!_builders.TryGet(key, out builders))
						{
							var plugins = _plugins.GetAll(type);
							builders = plugins.Select(x => Create.Instance<IBuilder>(x)).ToList();
							_builders.Add(key, builders);
						}
					}
				}

				return builders.Select(x => x.Execute());
			}
		}

		private interface IInstanceFactoryResolver
		{
			bool CanHandle(Type type);
			IFactory GetInstanceFactory(Type type);
		}

		private interface IFactory
		{
		}

		private interface IBuilder
		{
			object Execute();
		}

		private interface IBuilderLookup
		{
			void Add(string key, IBuilder builder);
			void Add(string key, IEnumerable<IBuilder> builders);
			bool TryGet(string key, out IBuilder builder);
			bool TryGet(string key, out IEnumerable<IBuilder> builders);
		}
	}
}