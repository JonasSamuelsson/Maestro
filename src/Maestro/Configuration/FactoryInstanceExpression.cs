using Maestro.Internals;

namespace Maestro.Configuration
{
	internal class FactoryInstanceExpression<TInstance> : InstanceExpression<TInstance, IFactoryInstanceExpression<TInstance>>, IFactoryInstanceExpression<TInstance>
	{
		public FactoryInstanceExpression(ServiceDescriptor serviceDescriptor)
			: base(serviceDescriptor)
		{
		}

		internal override IFactoryInstanceExpression<TInstance> Parent => this;

		public IFactoryInstanceExpression<T> As<T>()
		{
			return new FactoryInstanceExpression<T>(ServiceDescriptor);
		}
	}
}