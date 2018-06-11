using Maestro.FactoryProviders;
using Maestro.FactoryProviders.Factories;
using Maestro.Utils;
using System;
using System.Linq;

namespace Maestro.TypeFactoryResolvers
{
	internal class EmptyEnumerableFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, string name, Context context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;

			if (!Reflector.IsGenericEnumerable(type, out var elementType)) return false;
			var factoryProviderType = typeof(FactoryProvider<>).MakeGenericType(elementType);
			factoryProvider = (IFactoryProvider)Activator.CreateInstance(factoryProviderType);
			return true;
		}

		private class FactoryProvider<T> : IFactoryProvider
		{
			public Factory GetFactory(Context context)
			{
				return new Factory<T>();
			}

			public IFactoryProvider MakeGeneric(Type[] genericArguments)
			{
				throw new NotSupportedException();
			}

			public Type GetInstanceType()
			{
				return null;
			}
		}

		private class Factory<T> : Factory
		{
			internal override object GetInstance(Context context)
			{
				return Enumerable.Empty<T>();
			}
		}
	}
}