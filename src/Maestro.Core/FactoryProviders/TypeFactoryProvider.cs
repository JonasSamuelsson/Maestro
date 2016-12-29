using System;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	class TypeFactoryProvider : IFactoryProvider
	{
		public TypeFactoryProvider(Type type)
		{
			Type = type;
		}

		public Type Type { get; }
		public ConstructorInfo Constructor { get; set; }

		public IFactory GetFactory(Context context)
		{
			var constructor = Constructor ?? GetConstructor(context);
			var activator = ConstructorInvokation.Get(constructor);
			return new Factory(activator);
		}

		public IFactoryProvider MakeGeneric(Type[] genericArguments)
		{
			var type = Type.MakeGenericType(genericArguments);
			return new TypeFactoryProvider(type);
		}

		private ConstructorInfo GetConstructor(Context context)
		{
			// todo - merge logic with ConcreteClosedClassFactoryProviderResolver
			var constructor = (from ctor in Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
									 let parameterTypes = ctor.GetParameters().Select(x => x.ParameterType)
									 orderby parameterTypes.Count() descending
									 where parameterTypes.All(t => context.Kernel.CanGetService(t, context))
									 select ctor).FirstOrDefault();

			if (constructor == null)
			{
				throw new InvalidOperationException("Can't find appropriate constructor to invoke.");
			}

			return constructor;
		}
	}
}