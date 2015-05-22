using System;
using Maestro.Internals;

namespace Maestro.FactoryProviders.Factories
{
	class LambdaFactory : IFactory
	{
		private readonly Func<IContext, object> _factoryMethod;

		public LambdaFactory(Func<IContext, object> factoryMethod)
		{
			_factoryMethod = factoryMethod;
		}

		public object GetInstance(Context context)
		{
			return _factoryMethod(context);
		}
	}
}