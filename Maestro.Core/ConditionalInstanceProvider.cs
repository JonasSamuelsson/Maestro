using Maestro.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class ConditionalInstanceProvider<T> : IProvider, IConditionalInstancePipelineBuilder<T>
	{
		private readonly List<PredicatedPipeline> _predicatedPipelines = new List<PredicatedPipeline>();
		private IPipeline _defaultPipeline;

		public ConditionalInstanceProvider(Action<IConditionalInstancePipelineBuilder<T>> action)
		{
			action(this);
		}

		bool IProvider.CanGet(IContext context)
		{
			IPipeline pipeline;
			return TryGetPipeline(context, out pipeline) && pipeline.CanGet(context);
		}

		object IProvider.Get(IContext context)
		{
			IPipeline pipeline;
			if (TryGetPipeline(context, out pipeline))
				return pipeline.Get(context);

			throw new InvalidOperationException();
		}

		private bool TryGetPipeline(IContext context, out IPipeline pipeline)
		{
			foreach (var predicatedPipeline in _predicatedPipelines.Where(x => x.Predicate(context)))
			{
				pipeline = predicatedPipeline.Pipeline;
				return true;
			}

			pipeline = _defaultPipeline;
			return pipeline != null;
		}

		private struct PredicatedPipeline
		{
			public PredicatedPipeline(Func<IContext, bool> predicate, IPipeline pipeline)
				: this()
			{
				Predicate = predicate;
				Pipeline = pipeline;
			}

			public Func<IContext, bool> Predicate { get; private set; }
			public IPipeline Pipeline { get; private set; }
		}

		public IPipelineSelector<T> If(Func<IContext, bool> predicate)
		{
			return new PipelineSelector<T>(x => _predicatedPipelines.Add(new PredicatedPipeline(predicate, x)));
		}

		public IPipelineSelector<T> Default
		{
			get { return new PipelineSelector<T>(x => _defaultPipeline = x); }
		}
	}
}