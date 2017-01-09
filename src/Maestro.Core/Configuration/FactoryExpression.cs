using System;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class FactoryExpression<T> : IFactoryExpression<T>
	{
		private readonly Type _type;
		private readonly string _name;
		private readonly ServiceDescriptorLookup _serviceDescriptors;

		public FactoryExpression(Type type, string name, ServiceDescriptorLookup serviceDescriptors)
		{
			_type = type;
			_name = name;
			_serviceDescriptors = serviceDescriptors;
		}

		public void Instance(T instance)
		{
			_serviceDescriptors.Add(new ServiceDescriptor
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
			var plugin = new ServiceDescriptor
			{
				Type = _type,
				Name = _name,
				FactoryProvider = new LambdaFactoryProvider(ctx => factory(ctx))
			};
			_serviceDescriptors.Add(plugin);
			return new FactoryInstanceExpression<TInstance>(plugin);
		}

		public ITypeInstanceExpression<TInstance> Type<TInstance>()
		{
			var plugin = new ServiceDescriptor
			{
				Type = _type,
				Name = _name,
				FactoryProvider = new TypeFactoryProvider(_type, _name)
			};
			_serviceDescriptors.Add(plugin);
			return new TypeInstanceExpression<TInstance>(plugin);
		}
	}
}