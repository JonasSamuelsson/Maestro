using Maestro.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class ConditionalInstanceProvider<T> : IProvider, IConditionalInstancePipelineBuilder<T>
	{
		private readonly List<PredicatedPipeline> _predicatedPipelines = new List<PredicatedPipeline>();
		private IPipelineEngine _defaultPipelineEngine;

		public ConditionalInstanceProvider(Action<IConditionalInstancePipelineBuilder<T>> action)
		{
			action(this);
		}

		bool IProvider.CanGet(IContext context)
		{
			IPipelineEngine pipelineEngine;
			return TryGetPipeline(context, out pipelineEngine) && pipelineEngine.CanGet(context);
		}

		object IProvider.Get(IContext context)
		{
			IPipelineEngine pipelineEngine;
			if (TryGetPipeline(context, out pipelineEngine))
				return pipelineEngine.Get(context);

			throw new InvalidOperationException();
		}

		public IProvider MakeGenericProvider(Type[] types)
		{
			throw new NotImplementedException();
		}

		private bool TryGetPipeline(IContext context, out IPipelineEngine pipelineEngine)
		{
			foreach (var predicatedPipeline in _predicatedPipelines.Where(x => x.Predicate(context)))
			{
				pipelineEngine = predicatedPipeline.PipelineEngine;
				return true;
			}

			pipelineEngine = _defaultPipelineEngine;
			return pipelineEngine != null;
		}

		private struct PredicatedPipeline
		{
			public PredicatedPipeline(Func<IContext, bool> predicate, IPipelineEngine pipelineEngine)
				: this()
			{
				Predicate = predicate;
				PipelineEngine = pipelineEngine;
			}

			public Func<IContext, bool> Predicate { get; private set; }
			public IPipelineEngine PipelineEngine { get; private set; }
		}

		public IProviderSelector<T> If(Func<IContext, bool> predicate)
		{
			return new ProviderSelector<T>(x => _predicatedPipelines.Add(new PredicatedPipeline(predicate, x)));
		}

		public IProviderSelector<T> Default
		{
			get { return new ProviderSelector<T>(x => _defaultPipelineEngine = x); }
		}
	}
}