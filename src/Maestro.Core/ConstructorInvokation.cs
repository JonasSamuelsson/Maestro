using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Maestro.Internals;

namespace Maestro
{
	class ConstructorInvokation
	{
		static readonly Dictionary<ConstructorInfo, Func<Context, object>> Cache = new Dictionary<ConstructorInfo, Func<Context, object>>();

		public static Func<Context, object> Get(ConstructorInfo constructor, IEnumerable<Func<Context, object>> dependencyProviders = null)
		{
			Func<Context, object> func;
			if (Cache.TryGetValue(constructor, out func)) return func;

			var parameters = constructor.GetParameters();
			dependencyProviders = (dependencyProviders
										  ?? parameters.Select(x => x.ParameterType)
															.Select(x => new Func<Context, object>(ctx => ctx.Kernel.GetDependency(x, ctx)))).ToList();

			if (dependencyProviders.Count() != parameters.Length)
			{
				throw new InvalidOperationException();
			}

			var parameterExpression = Expression.Parameter(typeof(Context), "ctx");
			var index = 0;
			var dependencyExpressions = new List<Expression>();
			foreach (var dependencyProvider in dependencyProviders)
			{
				var type = parameters[index++].ParameterType;
				Expression<Func<Context, object>> providerExpression = ctx => dependencyProvider(ctx);
				var invokeExpression = Expression.Invoke(providerExpression, parameterExpression);
				var castExpression = Expression.Convert(invokeExpression, type);
				dependencyExpressions.Add(castExpression);
			}
			var newExpressions = Expression.New(constructor, dependencyExpressions);
			return Cache[constructor] = Expression.Lambda<Func<Context, object>>(newExpressions, parameterExpression).Compile();
		}
	}
}
