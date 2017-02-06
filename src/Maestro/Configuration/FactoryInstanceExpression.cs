using System;
using Maestro.Interceptors;
using Maestro.Internals;

namespace Maestro.Configuration
{
	class FactoryInstanceExpression<TInstance> : InstanceExpression<TInstance, IFactoryInstanceExpression<TInstance>>, IFactoryInstanceExpression<TInstance>
	{
		public FactoryInstanceExpression(ServiceDescriptor serviceDescriptor)
			: base(serviceDescriptor)
		{
		}

		internal override IFactoryInstanceExpression<TInstance> Parent => this;

		public IFactoryInstanceExpression<TInstanceOut> Intercept<TInstanceOut>(IInterceptor interceptor) where TInstanceOut : TInstance
		{
			Intercept(interceptor);
			return new FactoryInstanceExpression<TInstanceOut>(ServiceDescriptor);
		}
	}
}