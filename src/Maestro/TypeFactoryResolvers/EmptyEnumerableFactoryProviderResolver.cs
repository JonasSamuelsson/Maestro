using System;
using System.Linq;
using Maestro.FactoryProviders;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;
using Maestro.Utils;

namespace Maestro.TypeFactoryResolvers
{
	internal class EmptyEnumerableFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, string name, IContext context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;

			Type elementType;
			if (!Reflector.IsGenericEnumerable(type, out elementType)) return false;
			var factoryProviderType = typeof(FactoryProvider<>).MakeGenericType(elementType);
			factoryProvider = (IFactoryProvider)Activator.CreateInstance(factoryProviderType);
			return true;
		}

		private class FactoryProvider<T> : IFactoryProvider
		{
			public IFactory GetFactory(Context context)
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

		private class Factory<T> : IFactory
		{
			public object GetInstance(IContext context)
			{
				return Enumerable.Empty<T>();
			}
		}
	}
}