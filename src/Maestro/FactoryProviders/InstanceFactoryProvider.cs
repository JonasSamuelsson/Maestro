using System;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	class InstanceFactoryProvider : IFactoryProvider
	{
		private readonly object _instance;

		public InstanceFactoryProvider(object instance)
		{
			_instance = instance;
		}

		public IFactory GetFactory(Context context)
		{
			return new Factory(_ => _instance);
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