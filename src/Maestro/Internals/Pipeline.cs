using Maestro.FactoryProviders.Factories;
using Maestro.Interceptors;
using Maestro.Lifetimes;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class Pipeline : IPipeline
	{
		private readonly IFactory _factory;
		private readonly int _interceptorCount;
		private readonly List<IInterceptor> _interceptors;
		private readonly ILifetime _lifetime;

		public Pipeline(PipelineType pipelineType, IFactory factory, List<IInterceptor> interceptors, ILifetime lifetime)
		{
			PipelineType = pipelineType;
			_factory = factory;
			_interceptorCount = interceptors.Count;
			_interceptors = interceptors;
			_lifetime = lifetime;
		}

		public PipelineType PipelineType { get; }

		public object Execute(Context context)
		{
			return _lifetime.Execute(context, GetInstance);
		}

		public object GetInstance(Context context)
		{
			var ctx = context;

			var instance = _factory.GetInstance(ctx);

			var interceptors = _interceptors;
			for (var i = 0; i < _interceptorCount; i++)
			{
				instance = interceptors[i].Execute(instance, context);
			}

			return instance;
		}
	}
}