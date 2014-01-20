using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Factories;
using Maestro.Interceptors;
using Maestro.Lifetimes;
using Maestro.Utils;

namespace Maestro
{
	internal class InstanceBuilder : IInstanceBuilder
	{
		private readonly IInstanceFactory _instanceFactory;
		private readonly List<IInterceptor> _interceptors;
		private ILifetime _lifetime;

		public InstanceBuilder(IInstanceFactory instanceFactory)
		{
			_instanceFactory = instanceFactory;
			_interceptors = new List<IInterceptor>();
			_lifetime = TransientLifetime.Instance;
		}

		public bool CanGet(IContext context)
		{
			return _instanceFactory.CanGet(context);
		}

		public object Get(IContext context)
		{
			var pipeline = new Pipeline(_interceptors, _instanceFactory, context);
			return _lifetime.Execute(context, pipeline);
		}

		public IInstanceBuilder MakeGenericPipelineEngine(Type[] types)
		{
			var instanceBuilder = new InstanceBuilder(_instanceFactory.MakeGeneric(types));
			instanceBuilder._interceptors.AddRange(_interceptors.Select(x => x.Clone()));
			instanceBuilder._lifetime = _lifetime.Clone();
			return instanceBuilder;
		}

		public void SetLifetime(ILifetime lifetime)
		{
			_lifetime = lifetime;
		}

		public void AddInterceptor(IInterceptor interceptor)
		{
			_interceptors.Add(interceptor);
		}

		public void GetConfiguration(DiagnosticsBuilder builder)
		{
			using (builder.Category(_instanceFactory))
			{
				foreach (var interceptor in _interceptors)
					builder.Item("interceptor : {0}", interceptor);
				builder.Item("lifetime : {0}", _lifetime);
			}
		}

		public IInstanceBuilder Clone()
		{
			var instanceBuilder = new InstanceBuilder(_instanceFactory.Clone());
			instanceBuilder._interceptors.AddRange(_interceptors);
			instanceBuilder._lifetime = _lifetime;
			return instanceBuilder;
		}

		private class Pipeline : IPipeline
		{
			private readonly List<IInterceptor> _interceptors;
			private readonly IInstanceFactory _instanceFactory;
			private readonly IContext _context;

			public Pipeline(List<IInterceptor> interceptors, IInstanceFactory instanceFactory, IContext context)
			{
				_interceptors = interceptors;
				_instanceFactory = instanceFactory;
				_context = context;
			}

			public object Execute()
			{
				var instance = _instanceFactory.Get(_context);

				if (_interceptors.Count == 0)
					return instance;

				for (var i = 0; i < _interceptors.Count; i++)
				{
					var interceptor = _interceptors[i];
					instance = interceptor.Execute(instance, _context);
				}

				return instance;
			}
		}
	}
}