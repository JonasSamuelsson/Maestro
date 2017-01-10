using System;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	class TypeFactoryProvider : IFactoryProvider
	{
		public TypeFactoryProvider(Type type, string name)
		{
			Type = type;
			Name = name;
		}

		public Type Type { get; }
		public string Name { get; }
		public ConstructorInfo Constructor { get; set; }

		public IFactory GetFactory(Context context)
		{
			var constructor = Constructor ?? GetConstructor(context);
			var activator = GetActivator(constructor, Name); // ConstructorInvokation.Get(constructor);
			return new Factory(activator);
		}

		private static Func<IContext, object> GetActivator(ConstructorInfo constructor, string name)
		{
			var dependencyProviders = constructor
				.GetParameters()
				.Select(x => new Func<IContext, object>(ctx => ctx.GetService(x.ParameterType, name)));

			return context =>
			{
				var dependencies = dependencyProviders.Select(x => x(context));
				return constructor.Invoke(dependencies.ToArray());
				//return Activator.CreateInstance(constructor.ReflectedType, dependencies.ToArray());
			};
		}

		public IFactoryProvider MakeGeneric(Type[] genericArguments)
		{
			var type = Type.MakeGenericType(genericArguments);
			return new TypeFactoryProvider(type, Name);
		}

		private ConstructorInfo GetConstructor(Context context)
		{
			// todo - merge logic with ConcreteClosedClassFactoryProviderResolver
			var constructor = (from ctor in Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
									 let parameterTypes = ctor.GetParameters().Select(x => x.ParameterType)
									 orderby parameterTypes.Count() descending
									 where parameterTypes.All(t => context.Kernel.CanGetService(t, Name, context))
									 select ctor).FirstOrDefault();

			if (constructor == null)
			{
				throw new InvalidOperationException("Can't find appropriate constructor to invoke.");
			}

			return constructor;
		}
	}
}