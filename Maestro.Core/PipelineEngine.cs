using Maestro.Interceptors;
using Maestro.Lifecycles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class PipelineEngine : IPipelineEngine
	{
		private readonly List<IInterceptor> _onActivateInterceptors;
		private ILifecycle _lifecycle;
		private readonly List<IInterceptor> _onCreateInterceptors;
		private readonly IProvider _provider;

		public PipelineEngine(IProvider provider)
		{
			_onActivateInterceptors = new List<IInterceptor>();
			_lifecycle = TransientLifecycle.Instance;
			_onCreateInterceptors = new List<IInterceptor>();
			_provider = provider;
		}

		public bool CanGet(IContext context)
		{
			return _provider.CanGet(context);
		}

		public object Get(IContext context)
		{
			var pipeline = new Pipeline(_onCreateInterceptors, _provider, context);
			var instance = _lifecycle.Execute(context, pipeline);
			return _onActivateInterceptors.Count == 0
				? instance
				: _onActivateInterceptors.Aggregate(instance, (current, interceptor) => interceptor.Execute(current, context));
		}

		public IPipelineEngine MakeGenericPipelineEngine(Type[] types)
		{
			return new PipelineEngine(_provider.MakeGenericProvider(types))
			{
				_lifecycle = _lifecycle.Clone()
			};
		}

		public void AddOnCreateInterceptor(IInterceptor interceptor)
		{
			_onCreateInterceptors.Add(interceptor);
		}

		public void SetLifecycle(ILifecycle lifecycle)
		{
			_lifecycle = lifecycle;
		}

		public void AddOnActivateInterceptor(IInterceptor interceptor)
		{
			_onActivateInterceptors.Add(interceptor);
		}

		private class Pipeline : IPipeline
		{
			private readonly List<IInterceptor> _interceptors;
			private readonly IProvider _provider;
			private readonly IContext _context;

			public Pipeline(List<IInterceptor> interceptors, IProvider provider, IContext context)
			{
				_interceptors = interceptors;
				_provider = provider;
				_context = context;
			}

			public object Execute()
			{
				var instance = _provider.Get(_context);
				return _interceptors.Count == 0
					? instance
					: _interceptors.Aggregate(instance, (current, interceptor) => interceptor.Execute(current, _context));
			}
		}
	}
}