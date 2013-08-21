using System;
using System.Collections.Generic;

namespace Maestro
{
	internal class PipelineEngine : IPipelineEngine
	{
		private readonly IProvider _provider;
		private readonly List<IPipelineItem> _pipelineItems;

		public PipelineEngine(IProvider provider)
		{
			_provider = provider;
			_pipelineItems = new List<IPipelineItem>();
		}

		public bool CanGet(IContext context)
		{
			return _provider.CanGet(context);
		}

		public object Get(IContext context)
		{
			return new Pipeline(context, _pipelineItems, _provider).Execute();
		}

		public IPipelineEngine MakeGenericPipelineEngine(Type[] types)
		{
			return new PipelineEngine(_provider.MakeGenericProvider(types));
		}

		public void SetLifecycle(ILifecycle lifecycle)
		{
			_pipelineItems.Add(lifecycle);
		}

		private class Pipeline : IPipeline
		{
			private readonly IContext _context;
			private readonly Queue<IPipelineItem> _pipelineItems;
			private readonly IProvider _provider;

			public Pipeline(IContext context, IEnumerable<IPipelineItem> pipelineItems, IProvider provider)
			{
				_context = context;
				_pipelineItems = new Queue<IPipelineItem>(pipelineItems);
				_provider = provider;
			}

			public object Execute()
			{
				return _pipelineItems.Count != 0
					? _pipelineItems.Dequeue().Process(_context, this)
					: _provider.Get(_context);
			}
		}
	}
}