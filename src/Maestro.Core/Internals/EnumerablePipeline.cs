using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Internals
{
	class EnumerablePipeline : IPipeline
	{
		private readonly IEnumerable<IPipeline> _pipelines;
		private readonly MethodInfo _genericCastMethod;

		public EnumerablePipeline(Type elementType, IEnumerable<IPipeline> pipelines)
		{
			var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
			_genericCastMethod = castMethod.MakeGenericMethod(elementType);
			_pipelines = pipelines;
		}

		public object Execute(Context context)
		{
			var instances = _pipelines.Select(x => x.Execute(context));
			return _genericCastMethod.Invoke(null, new object[] { instances });
		}
	}
}