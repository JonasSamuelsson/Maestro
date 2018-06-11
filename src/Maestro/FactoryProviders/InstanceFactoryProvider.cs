using Maestro.FactoryProviders.Factories;
using System;

namespace Maestro.FactoryProviders
{
	class InstanceFactoryProvider : IFactoryProvider
	{
		private readonly object _instance;

		public InstanceFactoryProvider(object instance)
		{
			_instance = instance;
		}

		public Factory GetFactory(Context context)
		{
			return new DelegatingFactory(_ => _instance);
		}

		public IFactoryProvider MakeGeneric(Type[] genericArguments)
		{
			throw new NotSupportedException();
		}

		public Type GetInstanceType()
		{
			return _instance.GetType();
		}

		public override string ToString()
		{
			return "Instance";
		}
	}
}