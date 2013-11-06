using Maestro.Interceptors;
using Maestro.Lifetimes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class ConditionalPipelineEngine : IPipelineEngine
	{
		private readonly List<Item> _conditionalPipelineEngines;
		private IPipelineEngine _defaultPipelineEngine;

		public ConditionalPipelineEngine()
		{
			_conditionalPipelineEngines = new List<Item>();
		}

		public void Add(IPipelineEngine pipelineEngine)
		{
			_defaultPipelineEngine = pipelineEngine;
		}

		public void Add(Func<IContext, bool> condition, IPipelineEngine pipelineEngine)
		{
			_conditionalPipelineEngines.Add(new Item(condition, pipelineEngine));
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

		public IPipelineEngine MakeGenericPipelineEngine(Type[] types)
		{
			var engine = new ConditionalPipelineEngine();
			foreach (var item in _conditionalPipelineEngines)
				engine.Add(item.IsMatch, item.PipelineEngine.MakeGenericPipelineEngine(types));
			if (_defaultPipelineEngine != null)
				engine.Add(_defaultPipelineEngine.MakeGenericPipelineEngine(types));
			return engine;
		}

		private bool TryGetPipeline(IContext context, out IPipelineEngine pipelineEngine)
		{
			foreach (var conditionalPipelineEngine in _conditionalPipelineEngines.Where(x => x.IsMatch(context)))
			{
				pipelineEngine = conditionalPipelineEngine.PipelineEngine;
				return true;
			}

			pipelineEngine = _defaultPipelineEngine;
			return pipelineEngine != null;
		}

		private struct Item
		{
			public Item(Func<IContext, bool> predicate, IPipelineEngine pipelineEngine)
				: this()
			{
				IsMatch = predicate;
				PipelineEngine = pipelineEngine;
			}

			public Func<IContext, bool> IsMatch { get; private set; }
			public IPipelineEngine PipelineEngine { get; private set; }
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
				var list = _conditionalPipelineEngines.Select((x, i) => new { index = i + 1, engine = x.PipelineEngine }).ToList();
				foreach (var item in list)
				{
					builder.Prefix("condition {0} : ", item.index);
					item.engine.GetConfiguration(builder);
				}
				if (_defaultPipelineEngine != null)
				{
					builder.Prefix("default : ");
					_defaultPipelineEngine.GetConfiguration(builder);
				}
			}
		}
	}
}