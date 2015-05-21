using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Internals.FactoryProviders
{
	class TypeFactoryProvider : IFactoryProvider
	{
		public TypeFactoryProvider(Type type)
		{
			Type = type;
		}

		public Type Type { get; }
		public ConstructorInfo Constructor { get; set; }

		public Func<Context, object> GetFactory(Context context)
		{
			var types = Constructor?.GetParameters().Select(p => p.ParameterType)
			            ?? GetTypes(context);
			return types == null ? delegate { throw new InvalidOperationException(); } : GetFactory(types);
		}

		private IEnumerable<Type> GetTypes(Context context)
		{
			return Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
			           .Select(x => new { ctor = x, parameterTypes = x.GetParameters().Select(p => p.ParameterType).ToList() })
			           .OrderByDescending(x => x.parameterTypes.Count)
			           .Where(x => x.parameterTypes.All(context.CanGetDependency))
			           .Select(x => x.parameterTypes)
			           .FirstOrDefault();
		}

		private Func<Context, object> GetFactory(IEnumerable<Type> types)
		{
			return ctx =>
			       {
				       var dependencies = types.Select(ctx.GetDependency).ToArray();
				       return Activator.CreateInstance(Type, dependencies);
			       };
		}
	}
}