using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro
{
	class ConstructorInvokation
	{
		private static readonly ConcurrentDictionary<string, Func<IContext, object>> Cache = new ConcurrentDictionary<string, Func<IContext, object>>();

		public static Func<IContext, object> Create(ConstructorInfo constructor, string serviceName, IReadOnlyList<Func<IContext, object>> parameterProviders)
		{
			var key = $"{constructor.GetHashCode()}-{serviceName}";

			Func<IContext, object> activator;
			if (Cache.TryGetValue(key, out activator)) return activator;

			var parameters = constructor.GetParameters();

			if (parameterProviders.Count != parameters.Length)
			{
				throw new InvalidOperationException();
			}

			var parameterExpression = Expression.Parameter(typeof(IContext), "ctx");
			var index = 0;
			var factoryExpressions = new List<Expression>();
			foreach (var parameterProvider in parameterProviders)
			{
				var parameterType = parameters[index++].ParameterType;
				Expression<Func<IContext, object>> providerExpression = ctx => parameterProvider(ctx);
				var invokeExpression = Expression.Invoke(providerExpression, parameterExpression);
				var castExpression = Expression.Convert(invokeExpression, parameterType);
				factoryExpressions.Add(castExpression);
			}
			var newExpressions = Expression.New(constructor, factoryExpressions);
			return Cache[key] = Expression.Lambda<Func<IContext, object>>(newExpressions, parameterExpression).Compile();
		}
	}
}
