using System;
using System.Linq;
using System.Threading;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	class LazyFactoryResolver : ITypeFactoryResolver
	{
		public bool TryGet(Type type, Context context, out Pipeline pipeline)
		{
			pipeline = null;
			if (!type.IsConcreteClassClosing(typeof(Lazy<>))) return false;
			var typeArgument = type.GetGenericArguments().Single();
			if (!context.Kernel.CanGetDependency(typeArgument, context)) return false;

			pipeline = new Pipeline
			{
				Plugin = new Plugin
				{
					Type = type,
					Name = context.Name,
					FactoryProvider = new TypeFactoryProvider(type)
					{
						Dependencies = { { typeof(LazyThreadSafetyMode), LazyThreadSafetyMode.ExecutionAndPublication } }
					}
				}
			};
			return true;
		}
	}
}