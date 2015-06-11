using System;
using System.Collections.Generic;
using System.Linq;
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

			if (dependencyProviders == null)
			{
				var parameters = constructor.GetParameters();
				dependencyProviders = parameters.Select(x => x.ParameterType)
														  .Select(x => new Func<Context, object>(ctx => ctx.Kernel.GetDependency(x, ctx)));
			}

			return ctx => constructor.Invoke(dependencyProviders.Select(x => x(ctx)).ToArray()); // todo perf
		}
	}
}
