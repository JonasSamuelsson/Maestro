using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	class ComposedPipeline : IPipeline
	{
		private readonly List<IPipeline> _pipelines = new List<IPipeline>();
		private readonly IPipelineEngine _pipelineEngine;

		public ComposedPipeline(Type elementType)
		{
			var pipelineEngineType = typeof(PipelineEngine<>).MakeGenericType(elementType);
			_pipelineEngine = (IPipelineEngine)Activator.CreateInstance(pipelineEngineType);
		}

		public PipelineType PipelineType => PipelineType.Services;

		public void Add(IPipeline pipeline)
		{
			_pipelines.Add(pipeline);
		}

		public bool Any()
		{
			return _pipelines.Count != 0;
		}

		public object Execute(Context context)
		{
			return _pipelineEngine.Execute(_pipelines, context);
		}

		interface IPipelineEngine
		{
			object Execute(List<IPipeline> pipelines, Context context);
		}

		class PipelineEngine<T> : IPipelineEngine
		{
			public object Execute(List<IPipeline> pipelines, Context context)
			{
				var instances = new List<T>(pipelines.Count);

				// using for over foreach for better performance
				// ReSharper disable once ForCanBeConvertedToForeach
				for (var index = 0; index < pipelines.Count; index++)
				{
					var pipeline = pipelines[index];

					switch (pipeline.PipelineType)
					{
						case PipelineType.Service:
							instances.Add((T)pipeline.Execute(context));
							break;
						case PipelineType.Services:
							instances.AddRange((IEnumerable<T>)pipeline.Execute(context));
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				return instances;
			}
		}
	}
}