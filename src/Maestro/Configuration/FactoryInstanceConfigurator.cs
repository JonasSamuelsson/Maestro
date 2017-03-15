using Maestro.Internals;

namespace Maestro.Configuration
{
	class FactoryInstanceConfigurator<TInstance> : InstanceConfigurator<TInstance, IFactoryInstanceConfigurator<TInstance>>, IFactoryInstanceConfigurator<TInstance>
	{
		public FactoryInstanceConfigurator(ServiceDescriptor serviceDescriptor)
			: base(serviceDescriptor)
		{
		}

		internal override IFactoryInstanceConfigurator<TInstance> Parent => this;

		public IFactoryInstanceConfigurator<T> As<T>()
		{
			return new FactoryInstanceConfigurator<T>(ServiceDescriptor);
		}
	}
}