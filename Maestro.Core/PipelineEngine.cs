using Maestro.Lifecycles;
using System;

namespace Maestro
{
	internal class PipelineEngine : IPipelineEngine
	{
		private readonly IProvider _provider;
		private ILifecycle _lifecycle;

		public PipelineEngine(IProvider provider)
		{
			_provider = provider;
			_lifecycle = TransientLifecycle.Instance;
		}

		public bool CanGet(IContext context)
		{
			return _provider.CanGet(context);
		}

		public object Get(IContext context)
		{
			return _lifecycle.Process(context, new Pipeline(_provider, context));
		}

		public IPipelineEngine MakeGenericPipelineEngine(Type[] types)
		{
			return new PipelineEngine(_provider.MakeGenericProvider(types))
			{
				_lifecycle = _lifecycle.Clone()
			};
		}

		public void SetLifecycle(ILifecycle lifecycle)
		{
			_lifecycle = lifecycle;
		}

		private class Pipeline : IPipeline
		{
			private readonly IContext _context;
			private readonly IProvider _provider;

			public Pipeline(IProvider provider, IContext context)
			{
				_context = context;
				_provider = provider;
			}

			public object Execute()
			{
				return _provider.Get(_context);
			}
		}
	}
}