using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Maestro.FactoryProviders;
using Maestro.Utils;

namespace Maestro.TypeFactoryResolvers
{
	internal class ConcreteClosedClassFactoryProviderResolver : IFactoryProviderResolver
	{
		public bool TryGet(Type type, string name, IContext context, out IFactoryProvider factoryProvider)
		{
			factoryProvider = null;

			if (type.IsArray) return false;
			if (Reflector.IsPrimitive(type)) return false;
			if (!type.IsConcreteClosedClass()) return false;
			if (type.IsConcreteClassClosing(typeof(Func<>))) return false;
			if (type.IsConcreteClassClosing(typeof(Lazy<>))) return false;

			factoryProvider = GetFactoryProviders(type, name, context).FirstOrDefault();

			return factoryProvider != null;
		}

		private static IEnumerable<TypeFactoryProvider> GetFactoryProviders(Type type, string name, IContext context)
		{
			return from ctor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
					 let parameters = ctor.GetParameters()
					 orderby parameters.Length descending
					 where parameters.All(p => context.CanGetService(p.ParameterType, name))
					 select new TypeFactoryProvider(type, name) { Constructor = ctor };
		}
	}
}