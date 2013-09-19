using Maestro.Interceptors;
using Maestro.Lifetimes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro
{
	internal class PipelineEngine : IPipelineEngine
	{
		private readonly List<IInterceptor> _onActivateInterceptors;
		private ILifetime _lifetime;
		private readonly List<IInterceptor> _onCreateInterceptors;
		private readonly IProvider _provider;

		public PipelineEngine(IProvider provider)
		{
			_onActivateInterceptors = new List<IInterceptor>();
			_lifetime = TransientLifetime.Instance;
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
			var instance = _lifetime.Execute(context, pipeline);
			return _onActivateInterceptors.Count == 0
				? instance
				: _onActivateInterceptors.Aggregate(instance, (current, interceptor) => interceptor.Execute(current, context));
		}

		public IPipelineEngine MakeGenericPipelineEngine(Type[] types)
		{
			var engine = new PipelineEngine(_provider.MakeGenericProvider(types));
			engine._onCreateInterceptors.AddRange(_onCreateInterceptors.Select(x => x.Clone()));
			engine._lifetime = _lifetime.Clone();
			engine._onActivateInterceptors.AddRange(_onActivateInterceptors.Select(x => x.Clone()));
			return engine;
		}

		public void AddOnCreateInterceptor(IInterceptor interceptor)
		{
			_onCreateInterceptors.Add(interceptor);
		}

		public void SetLifetime(ILifetime lifetime)
		{
			_lifetime = lifetime;
		}

		public void AddOnActivateInterceptor(IInterceptor interceptor)
		{
			_onActivateInterceptors.Add(interceptor);
		}

		public void PrintConfiguration(ConfigOutputBuilder builder)
		{
			using (builder.Category(_provider))
			{
				foreach (var interceptor in _onActivateInterceptors)
					builder.Item("on activate : {0}", interceptor);
				builder.Item("lifetime : {0}", _lifetime);
				foreach (var interceptor in _onCreateInterceptors)
					builder.Item("on create : {0}", interceptor);
			}
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