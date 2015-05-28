using System;
using System.Collections.Generic;
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
			Dependencies = new Dictionary<Type, object>();
		}

		public Type Type { get; }
		public ConstructorInfo Constructor { get; set; }
		public Dictionary<Type, object> Dependencies { get; }

		public IFactory GetFactory(Context context)
		{
			var ctorParameterTypes = Constructor?.GetParameters().Select(p => p.ParameterType)
											 ?? GetTypes(context);
			if (ctorParameterTypes == null) throw new InvalidOperationException("Can't find appropriate constructor to invoke.");
			return new TypeFactory(Type, ctorParameterTypes, Dependencies);
		}

		private IEnumerable<Type> GetTypes(Context context)
		{
			return Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
						  .Select(x => new { ctor = x, parameterTypes = x.GetParameters().Select(p => p.ParameterType).ToList() })
						  .OrderByDescending(x => x.parameterTypes.Count)
						  .Where(x => x.parameterTypes.All(t => Dependencies.ContainsKey(t) || context.Kernel.CanGetDependency(t, context)))
						  .Select(x => x.parameterTypes)
						  .FirstOrDefault();
		}
	}
}