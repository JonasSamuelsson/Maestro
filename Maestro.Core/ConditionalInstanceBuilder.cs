using Maestro.Interceptors;
using Maestro.Lifetimes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class ConditionalInstanceBuilder : IInstanceBuilder
	{
		private readonly List<Item> _conditionalPipelineEngines;
		private IInstanceBuilder _defaultInstanceBuilder;

		public ConditionalInstanceBuilder()
		{
			_conditionalPipelineEngines = new List<Item>();
		}

		public void Add(IInstanceBuilder instanceBuilder)
		{
			_defaultInstanceBuilder = instanceBuilder;
		}

		public void Add(Func<IContext, bool> condition, IInstanceBuilder instanceBuilder)
		{
			_conditionalPipelineEngines.Add(new Item(condition, instanceBuilder));
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

		public IInstanceBuilder MakeGenericPipelineEngine(Type[] types)
		{
			var engine = new ConditionalInstanceBuilder();
			foreach (var item in _conditionalPipelineEngines)
				engine.Add(item.IsMatch, item.InstanceBuilder.MakeGenericPipelineEngine(types));
			if (_defaultInstanceBuilder != null)
				engine.Add(_defaultInstanceBuilder.MakeGenericPipelineEngine(types));
			return engine;
		}

		private bool TryGetPipeline(IContext context, out IInstanceBuilder instanceBuilder)
		{
			foreach (var conditionalPipelineEngine in _conditionalPipelineEngines.Where(x => x.IsMatch(context)))
			{
				instanceBuilder = conditionalPipelineEngine.InstanceBuilder;
				return true;
			}

			instanceBuilder = _defaultInstanceBuilder;
			return instanceBuilder != null;
		}

		private struct Item
		{
			public Item(Func<IContext, bool> predicate, IInstanceBuilder instanceBuilder)
				: this()
			{
				IsMatch = predicate;
				InstanceBuilder = instanceBuilder;
			}

			public Func<IContext, bool> IsMatch { get; private set; }
			public IInstanceBuilder InstanceBuilder { get; private set; }
		}

		public void SetLifetime(ILifetime lifetime)
		{
			throw new NotSupportedException();
		}

		public void AddInterceptor(IInterceptor interceptor)
		{
			throw new NotSupportedException();
		}

		public void GetConfiguration(DiagnosticsBuilder builder)
		{
			using (builder.Category("conditional instance"))
			{
				var list = _conditionalPipelineEngines.Select((x, i) => new { index = i + 1, engine = x.InstanceBuilder }).ToList();
				foreach (var item in list)
				{
					builder.Prefix("condition {0} : ", item.index);
					item.engine.GetConfiguration(builder);
				}
				if (_defaultInstanceBuilder != null)
				{
					builder.Prefix("default : ");
					_defaultInstanceBuilder.GetConfiguration(builder);
				}
			}
		}
	}
}