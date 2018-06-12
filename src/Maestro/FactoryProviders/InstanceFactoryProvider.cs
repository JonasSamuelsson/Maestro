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