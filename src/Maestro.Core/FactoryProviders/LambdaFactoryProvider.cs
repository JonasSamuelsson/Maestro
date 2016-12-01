using System;
using System.Collections.Generic;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	class LambdaFactoryProvider : IFactoryProvider
	{
		private readonly Func<IContext, object> _factory;

		public LambdaFactoryProvider(Func<IContext, object> factory)
		{
			_factory = factory;
		}

		public IFactory GetFactory(Context context)
		{
			return new Factory(_factory);
		}

		public IFactoryProvider MakeGeneric(Type[] genericArguments)
		{
			throw new NotSupportedException();
		}
	}
}