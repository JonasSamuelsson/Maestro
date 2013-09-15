using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class ConditionalInstanceProvider : IProvider
	{
		private readonly List<ConditionalPipelineEngine> _conditionalPipelineEngines;
		private IPipelineEngine _defaultPipelineEngine;

		public ConditionalInstanceProvider()
		{
			_conditionalPipelineEngines = new List<ConditionalPipelineEngine>();
		}

		public void Add(IPipelineEngine pipelineEngine)
		{
			_defaultPipelineEngine = pipelineEngine;
		}

		public void Add(Func<IContext, bool> condition, IPipelineEngine pipelineEngine)
		{
			_conditionalPipelineEngines.Add(new ConditionalPipelineEngine(condition, pipelineEngine));
		}

		public bool CanGet(IContext context)
		{
			IPipelineEngine engine;
			return TryGetPipeline(context, out engine) && engine.CanGet(context);
		}

		public object Get(IContext context)
		{
			IPipelineEngine engine;
			if (TryGetPipeline(context, out engine)) return engine.Get(context);
			throw new Exception("Conditional instance not found.");
		}

		public IProvider MakeGenericProvider(Type[] types)
		{
			var provider = new ConditionalInstanceProvider();
			if (_defaultPipelineEngine != null)
				provider._defaultPipelineEngine = _defaultPipelineEngine.MakeGenericPipelineEngine(types);
			foreach (var item in _conditionalPipelineEngines)
			{
				var genericPipelineEngine = item.PipelineEngine.MakeGenericPipelineEngine(types);
				var conditionalPipelineEngine = new ConditionalPipelineEngine(item.Condition, genericPipelineEngine);
				provider._conditionalPipelineEngines.Add(conditionalPipelineEngine);
			}
			return provider;
		}

		private bool TryGetPipeline(IContext context, out IPipelineEngine pipelineEngine)
		{
			foreach (var conditionalPipelineEngine in _conditionalPipelineEngines.Where(x => x.Condition(context)))
			{
				pipelineEngine = conditionalPipelineEngine.PipelineEngine;
				return true;
			}

			pipelineEngine = _defaultPipelineEngine;
			return pipelineEngine != null;
		}

		private struct ConditionalPipelineEngine
		{
			public ConditionalPipelineEngine(Func<IContext, bool> condition, IPipelineEngine pipelineEngine)
				: this()
			{
				Condition = condition;
				PipelineEngine = pipelineEngine;
			}

			public Func<IContext, bool> Condition { get; private set; }
			public IPipelineEngine PipelineEngine { get; private set; }
		}
	}
}