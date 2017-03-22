using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Maestro.FactoryProviders;

namespace Maestro.TypeFactoryResolvers
{
	class LazyFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, string name, IContext context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;
			if (!type.IsConcreteClassClosing(typeof(Lazy<>))) return false;
			var typeArgument = type.GetGenericArguments().Single();
			if (!context.CanGetService(typeArgument, name)) return false;

			var funcType = typeof(Func<>).MakeGenericType(typeArgument);
			var constructor = (from ctor in type.GetConstructors()
									 let parameters = ctor.GetParameters()
									 where parameters.Length == 2
									 where parameters[0].ParameterType == funcType
									 where parameters[1].ParameterType == typeof(LazyThreadSafetyMode)
									 select ctor).First();
			var param1 = new Func<IContext, object>(ctx => ctx.GetService(funcType, name));
			var param2 = new Func<IContext, object>(_ => LazyThreadSafetyMode.ExecutionAndPublication);
			var factories = new[] { param1, param2 };
			var innerActivator = ConstructorInvokation.Create(constructor, factories);
			factoryProvider = new LambdaFactoryProvider(ctx => innerActivator(factories, ctx));
			return true;
		}
	}
}