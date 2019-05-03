using Maestro.Diagnostics;
using System;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class CompositePipeline : Pipeline
	{
		private readonly List<Pipeline> _pipelines = new List<Pipeline>();
		private readonly PipelineEngine _pipelineEngine;

		public CompositePipeline(Type elementType)
		{
			var pipelineEngineType = typeof(PipelineEngine<>).MakeGenericType(elementType);
			_pipelineEngine = (PipelineEngine)Activator.CreateInstance(pipelineEngineType);
		}

		public void Add(Pipeline pipeline)
		{
			_pipelines.Add(pipeline);
		}

		internal override object Execute(Context context)
		{
			return _pipelineEngine.Execute(_pipelines, context);
		}

		internal override void Populate(List<PipelineService> services)
		{
			services.Add(new PipelineService { Provider = "service[]" });
			_pipelines.ForEach(x => x.Populate(services));
		}

		private abstract class PipelineEngine
		{
			internal abstract object Execute(List<Pipeline> pipelines, Context context);
		}

		private class PipelineEngine<T> : PipelineEngine
		{
			internal override object Execute(List<Pipeline> pipelines, Context context)
			{
				var instances = new List<T>(pipelines.Count);

				// ReSharper disable once ForCanBeConvertedToForeach
				// ReSharper disable once LoopCanBeConvertedToQuery
				for (var index = 0; index < pipelines.Count; index++)
				{
					var pipeline = pipelines[index];
					instances.Add((T)pipeline.Execute(context));
				}

				return instances;
			}
		}
	}
}