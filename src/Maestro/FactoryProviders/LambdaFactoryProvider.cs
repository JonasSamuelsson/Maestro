using System;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	class LambdaFactoryProvider : IFactoryProvider
	{
		private readonly Func<Context, object> _factory;

		public LambdaFactoryProvider(Func<Context, object> factory)
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

		public Type GetInstanceType()
		{
			return null;
		}

		public override string ToString()
		{
			return "Factory";
		}
	}
}