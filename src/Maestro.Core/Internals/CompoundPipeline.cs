using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Maestro.Internals
{
	class CompoundPipeline : IPipeline
	{
		private readonly List<IPipeline> _pipelines = new List<IPipeline>();
		private readonly MethodInfo _genericCastMethod;

		public CompoundPipeline(Type elementType)
		{
			var castMethod = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
			_genericCastMethod = castMethod.MakeGenericMethod(elementType);
		}

		public void Add(IPipeline pipeline)
		{
			_pipelines.Add(pipeline);
		}

		public bool Any()
		{
			return _pipelines.Any();
		}

		public object Execute(Context context)
		{
			var result = new List<object>();

			foreach (var pipeline in _pipelines)
			{
				var pipelineType = pipeline.GetType();

				if (pipelineType == typeof(Pipeline))
				{
					var instance = pipeline.Execute(context);
					result.Add(instance);
				}
				else if (pipelineType == typeof(EnumerablePipeline))
				{
					var instance = pipeline.Execute(context);
					var instances = ((IEnumerable)instance).Cast<object>();
					result.AddRange(instances);
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			return _genericCastMethod.Invoke(null, new object[] { result });
		}
	}
}