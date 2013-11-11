using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Factories
{
	internal class ConditionalInstanceFactory : IInstanceFactory
	{
		private readonly List<PredicatedPipelineEngine> _conditionalPipelineEngines;
		private IInstanceBuilder _defaultInstanceBuilder;

		public ConditionalInstanceFactory()
		{
			_conditionalPipelineEngines = new List<PredicatedPipelineEngine>();
		}

		public void Add(IInstanceBuilder instanceBuilder)
		{
			_defaultInstanceBuilder = instanceBuilder;
		}

		public void Add(Func<IContext, bool> condition, IInstanceBuilder instanceBuilder)
		{
			_conditionalPipelineEngines.Add(new PredicatedPipelineEngine(condition, instanceBuilder));
		}

		public bool CanGet(IContext context)
		{
			IInstanceBuilder engine;
			return TryGetPipeline(context, out engine) && engine.CanGet(context);
		}

		public object Get(IContext context)
		{
			IInstanceBuilder engine;
			if (TryGetPipeline(context, out engine)) return engine.Get(context);
			throw new Exception("Conditional instance not found.");
		}

		public IInstanceFactory MakeGenericInstanceFactory(Type[] types)
		{
			var provider = new ConditionalInstanceFactory();
			if (_defaultInstanceBuilder != null)
				provider._defaultInstanceBuilder = _defaultInstanceBuilder.MakeGenericPipelineEngine(types);
			foreach (var item in _conditionalPipelineEngines)
			{
				var genericPipelineEngine = item.InstanceBuilder.MakeGenericPipelineEngine(types);
				var conditionalPipelineEngine = new PredicatedPipelineEngine(item.Condition, genericPipelineEngine);
				provider._conditionalPipelineEngines.Add(conditionalPipelineEngine);
			}
			return provider;
		}

		private bool TryGetPipeline(IContext context, out IInstanceBuilder instanceBuilder)
		{
			foreach (var conditionalPipelineEngine in _conditionalPipelineEngines.Where(x => x.Condition(context)))
			{
				instanceBuilder = conditionalPipelineEngine.InstanceBuilder;
				return true;
			}

			instanceBuilder = _defaultInstanceBuilder;
			return instanceBuilder != null;
		}

		private struct PredicatedPipelineEngine
		{
			public PredicatedPipelineEngine(Func<IContext, bool> condition, IInstanceBuilder instanceBuilder)
				: this()
			{
				Condition = condition;
				InstanceBuilder = instanceBuilder;
			}

			public Func<IContext, bool> Condition { get; private set; }
			public IInstanceBuilder InstanceBuilder { get; private set; }
		}
	}
}