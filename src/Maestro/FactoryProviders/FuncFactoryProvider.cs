using Maestro.FactoryProviders.Factories;
using System;

namespace Maestro.FactoryProviders
{
	internal class FuncFactoryProvider : IFactoryProvider
	{
		private readonly Func<Context, object> _factory;

		public FuncFactoryProvider(Func<Context, object> factory)
		{
			_factory = factory;
		}

		public Factory GetFactory(Context context)
		{
			return new FuncFactory(_factory);
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