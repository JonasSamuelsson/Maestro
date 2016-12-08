using System;
using System.Linq;
using System.Threading;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	class LazyFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, Context context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;
			if (!type.IsConcreteClassClosing(typeof(Lazy<>))) return false;
			var typeArgument = type.GetGenericArguments().Single();
			if (!context.Kernel.CanGetService(typeArgument, context)) return false;

			var funcType = typeof(Func<>).MakeGenericType(typeArgument);
			var constructor = (from ctor in type.GetConstructors()
									 let parameters = ctor.GetParameters()
									 where parameters.Length == 2
									 where parameters[0].ParameterType == funcType
									 where parameters[1].ParameterType == typeof(LazyThreadSafetyMode)
									 select ctor).First();
			var param1 = new Func<Context, object>(ctx => ctx.GetService(funcType));
			var param2 = new Func<Context, object>(_ => LazyThreadSafetyMode.ExecutionAndPublication);
			var activator = ConstructorInvokation.Get(constructor, new[] { param1, param2 });
			factoryProvider = new LambdaFactoryProvider(ctx => activator((Context)ctx));
			return true;
		}
	}
}