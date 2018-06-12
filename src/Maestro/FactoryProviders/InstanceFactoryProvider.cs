using Maestro.FactoryProviders.Factories;
using System;

namespace Maestro.FactoryProviders
{
	internal class InstanceFactoryProvider : IFactoryProvider
	{
		private readonly object _instance;

		public InstanceFactoryProvider(object instance)
		{
			_instance = instance;
		}

		public Factory GetFactory(Context context)
		{
			return new InstanceFactory(_instance);
		}

		public IFactoryProvider MakeGeneric(Type[] genericArguments)
		{
			throw new InvalidOperationException($"Can't create generic instance from instance of type '{_instance.GetType().FullName}'.");
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