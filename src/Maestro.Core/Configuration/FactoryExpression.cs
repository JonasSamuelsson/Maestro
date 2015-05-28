using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class FactoryExpression<T> : IFactoryExpression<T>
	{
		private readonly Type _type;
		private readonly string _name;
		private readonly PluginLookup _plugins;

		public FactoryExpression(Type type, string name, PluginLookup plugins)
		{
			_type = type;
			_name = name;
			_plugins = plugins;
		}

		public void Instance(T instance)
		{
			_plugins.Add(new Plugin
			             {
				             Type = _type,
				             Name = _name,
				             FactoryProvider = new InstanceFactoryProvider(instance)
			             });
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<TInstance> factory)
		{
			return Factory(_ => factory());
		}

		public IFactoryInstanceExpression<TInstance> Factory<TInstance>(Func<IContext, TInstance> factory)
		{
			var plugin = new Plugin
			             {
				             Type = _type,
				             Name = _name,
				             FactoryProvider = new LambdaFactoryProvider(ctx => factory(ctx))
			             };
			_plugins.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Type<TInstance>()
		{
			var plugin = new Plugin
			             {
				             Type = _type,
				             Name = _name,
				             FactoryProvider = new TypeFactoryProvider(_type)
			             };
			_plugins.Add(plugin);
			return new TypeInstanceExpression<TInstance>(plugin);
		}
	}
}