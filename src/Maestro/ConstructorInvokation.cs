using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro
{
	class ConstructorInvokation
	{
		private static readonly ConcurrentDictionary<ConstructorInfo, Func<IReadOnlyList<Func<IContext, object>>, IContext, object>> Cache = new ConcurrentDictionary<ConstructorInfo, Func<IReadOnlyList<Func<IContext, object>>, IContext, object>>();

		public static Func<IReadOnlyList<Func<IContext, object>>, IContext, object> Create(ConstructorInfo constructor, IReadOnlyList<Func<IContext, object>> factories)
		{

			Func<IReadOnlyList<Func<IContext, object>>, IContext, object> activator;
			if (Cache.TryGetValue(constructor, out activator)) return activator;

			var parameters = constructor.GetParameters();

			if (factories.Count != parameters.Length)
			{
				throw new InvalidOperationException();
			}

			var factoriesExpression = Expression.Parameter(typeof(IReadOnlyList<Func<IContext, object>>), "factories");
			var contextExpression = Expression.Parameter(typeof(IContext), "context");
			var typedValueExpressions = new List<Expression>();
			var property = typeof(IReadOnlyList<Func<IContext, object>>).GetProperty("Item");
			for (var index = 0; index < factories.Count; index++)
			{
				var indexExpression = Expression.Constant(index, typeof(int));
				var factoryExpression = Expression.Property(factoriesExpression, property, indexExpression);
				var valueExpression = Expression.Invoke(factoryExpression, contextExpression);
				var typedValueExpression = Expression.Convert(valueExpression, parameters[index].ParameterType);
				typedValueExpressions.Add(typedValueExpression);
			}
			var newExpression = Expression.New(constructor, typedValueExpressions);
			return Cache[constructor] = Expression.Lambda<Func<IReadOnlyList<Func<IContext, object>>, IContext, object>>(newExpression, factoriesExpression, contextExpression).Compile();
		}
	}
}
