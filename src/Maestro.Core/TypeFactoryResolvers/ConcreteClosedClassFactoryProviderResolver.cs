using System;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders;
using Maestro.Internals;

namespace Maestro.TypeFactoryResolvers
{
	internal class ConcreteClosedClassFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, Context context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;
			if (type == typeof(object)) return false;
			if (!type.IsConcreteClosedClass()) return false;
			if (type.IsArray) return false;
			if (type.IsConcreteClassClosing(typeof(Func<>))) return false;
			if (type.IsConcreteClassClosing(typeof(Lazy<>))) return false;

			factoryProvider = (from ctor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
									 let parameters = ctor.GetParameters()
									 orderby parameters.Length descending
									 where parameters.All(p => context.Kernel.CanGetService(p.ParameterType, context))
									 select new TypeFactoryProvider(type) { Constructor = ctor }).FirstOrDefault();
			return factoryProvider != null;
		}
	}
}