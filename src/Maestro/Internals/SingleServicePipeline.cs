using Maestro.FactoryProviders.Factories;
using Maestro.Interceptors;
using Maestro.Lifetimes;
using System.Collections.Generic;

namespace Maestro.Internals
{
	internal class SingleServicePipeline : Pipeline
	{
		private readonly Factory _factory;
		private readonly int _interceptorCount;
		private readonly List<Interceptor> _interceptors;
		private readonly Lifetime _lifetime;

		internal SingleServicePipeline(Factory factory, List<Interceptor> interceptors, Lifetime lifetime)
		{
			_factory = factory;
			_interceptorCount = interceptors.Count;
			_interceptors = interceptors;
			_lifetime = lifetime;
		}

		internal override object Execute(Context context)
		{
			return _lifetime.Execute(context, GetInstance);
		}

		public object GetInstance(Context context)
		{
			var instance = _factory.GetInstance(context);

			var interceptors = _interceptors;
			for (var i = 0; i < _interceptorCount; i++)
			{
				instance = interceptors[i].Execute(instance, context);
			}

			return instance;
		}
	}
}