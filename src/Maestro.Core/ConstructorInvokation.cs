using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Maestro
{
	class ConstructorInvokation
	{
		static readonly Dictionary<ConstructorInfo, Func<IContext, object>> Cache = new Dictionary<ConstructorInfo, Func<IContext, object>>();

		public static Func<IContext, object> Get(ConstructorInfo constructor, IEnumerable<Func<IContext, object>> dependencyProviders = null)
		{
			Func<IContext, object> func;
			if (Cache.TryGetValue(constructor, out func)) return func;

			var parameters = constructor.GetParameters();
			dependencyProviders = (dependencyProviders
										  ?? parameters.Select(x => x.ParameterType)
															.Select(x => new Func<IContext, object>(ctx => ctx.GetService(x)))).ToList();

			if (dependencyProviders.Count() != parameters.Length)
			{
				throw new InvalidOperationException();
			}

			var parameterExpression = Expression.Parameter(typeof(IContext), "ctx");
			var index = 0;
			var dependencyExpressions = new List<Expression>();
			foreach (var dependencyProvider in dependencyProviders)
			{
				var type = parameters[index++].ParameterType;
				Expression<Func<IContext, object>> providerExpression = ctx => dependencyProvider(ctx);
				var invokeExpression = Expression.Invoke(providerExpression, parameterExpression);
				var castExpression = Expression.Convert(invokeExpression, type);
				dependencyExpressions.Add(castExpression);
			}
			var newExpressions = Expression.New(constructor, dependencyExpressions);
			return Cache[constructor] = Expression.Lambda<Func<IContext, object>>(newExpressions, parameterExpression).Compile();
		}
	}
}
