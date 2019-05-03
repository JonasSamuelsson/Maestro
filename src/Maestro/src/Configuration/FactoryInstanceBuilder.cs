using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class FactoryInstanceBuilder<TInstance> : InstanceBuilder<TInstance, IFactoryInstanceBuilder<TInstance>>, IFactoryInstanceBuilder<TInstance>
	{
		public FactoryInstanceBuilder(ServiceDescriptor serviceDescriptor)
			: base(serviceDescriptor)
		{
		}

		internal override IFactoryInstanceBuilder<TInstance> Parent => this;

		public IFactoryInstanceBuilder<T> As<T>()
		{
			return new FactoryInstanceBuilder<T>(ServiceDescriptor);
		}
	}
}