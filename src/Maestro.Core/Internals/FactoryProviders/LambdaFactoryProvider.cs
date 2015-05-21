using System;

namespace Maestro.Internals.FactoryProviders
{
	class LambdaFactoryProvider : IFactoryProvider
	{
		private readonly Func<Context, object> _factory;

		public LambdaFactoryProvider(Func<Context, object> factory)
		{
			_factory = factory;
		}

		public Func<Context, object> GetFactory(Context context)
		{
			return _factory;
		}
	}
}