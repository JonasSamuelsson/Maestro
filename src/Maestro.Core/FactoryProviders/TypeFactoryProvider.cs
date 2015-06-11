using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Maestro.FactoryProviders.Factories;
using Maestro.Internals;

namespace Maestro.FactoryProviders
{
	class TypeFactoryProvider : IFactoryProvider
	{
		private readonly Dictionary<ConstructorInfo, Func<Context, object>> _cache = new Dictionary<ConstructorInfo, Func<Context, object>>();

		public TypeFactoryProvider(Type type)
		{
			Type = type;
		}

		public Type Type { get; }
		public ConstructorInfo Constructor { get; set; }

		public IFactory GetFactory(Context context)
		{
			var constructor = Constructor ?? GetConstructor(context);

			Func<Context, object> activator;
			if (!_cache.TryGetValue(constructor, out activator))
			{
				//var dependencyProviders = constructor.GetParameters()
				//									  .Select(x => GetDependencyProvider(x.ParameterType))
				//									  .ToList();

				//_cache[constructor] = activator = GetActivator(constructor, dependencyProviders);
				_cache[constructor] = activator = ctx =>
															 {
																 var dependencies = constructor.GetParameters()
																										 .Select(x => x.ParameterType)
																										 .Select(x => ctx.Kernel.GetDependency(x, ctx));
																 return constructor.Invoke(dependencies.ToArray());
															 };
			}

			return new Factory(activator);
		}

		//private DependencyProvider GetDependencyProvider(Type type)
		//{
		//	return new DependencyProvider
		//	{
		//		Type = type,
		//		Provider = ctx => ctx.Kernel.GetDependency(type, ctx)
		//	};
		//}

		//private Func<Context, object> GetActivator(ConstructorInfo constructor, IEnumerable<DependencyProvider> dependencyProviders)
		//{
		//	var parameterExpression = Expression.Parameter(typeof(Context), "ctx");
		//	var dependencyExpressions = dependencyProviders.Select(x => Expression.Convert(Expression.Call(x.Provider.Method, parameterExpression), x.Type));
		//	var newExpression = Expression.New(constructor, dependencyExpressions);
		//	return Expression.Lambda<Func<Context, object>>(newExpression, parameterExpression).Compile();
		//}

		private ConstructorInfo GetConstructor(Context context)
		{
			var constructor = (from ctor in Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
									 let parameterTypes = ctor.GetParameters().Select(x => x.ParameterType)
									 orderby parameterTypes.Count() descending
									 where parameterTypes.All(t => context.Kernel.CanGetDependency(t, context))
									 select ctor).FirstOrDefault();

			if (constructor == null)
			{
				throw new InvalidOperationException("Can't find appropriate constructor to invoke.");
			}

			return constructor;
		}

		class DependencyProvider
		{
			public Type Type { get; set; }
			public Func<Context, object> Provider { get; set; }
		}
	}
}